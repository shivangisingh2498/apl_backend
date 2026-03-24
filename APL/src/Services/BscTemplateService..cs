using APL.Data;
using APL.Entities;
using APL.Models;
using APL.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Xml.Linq;
namespace APL.Services
{
    public class BscTemplateService : IBscTemplateService
    {
        private readonly AplDbContext _db;
        private readonly IDbContextFactory<AplDbContext> _contextFactory;

        public BscTemplateService(AplDbContext db, IDbContextFactory<AplDbContext> contextFactory)
        {
            _db = db;
            _contextFactory = contextFactory;
        }

        public async Task<StationDepartmentDto> GetBscList()
        {
            List<DepartmentMasterDto> result = await _db.tbl_department_master
                .AsNoTracking().Select(x => new DepartmentMasterDto
                {
                    id = x.id,
                    department = x.department,
                    departmentName = x.departmentname,
                    isActive = x.isactive
                })
               .AsNoTracking()
               .OrderBy(f => f.id)
               .ToListAsync();

            List<StationDto> stationList = await _db.tbl_station_master
             .AsNoTracking().Select(x => new StationDto
             {
                 id = x.id,
                 station = x.station,
                 isActive = x.isactive
             })
            .AsNoTracking()
            .OrderBy(f => f.id)
            .ToListAsync();


            return new StationDepartmentDto
            {
                departmentList = result,
                stationList = stationList

            };
        }

        public async Task<CreateTemplateDropdownDto> GetBscTemplateDropdown()
        {


            AplDbContext context1 = _contextFactory.CreateDbContext();
            AplDbContext context2 = _contextFactory.CreateDbContext();
            AplDbContext context3 = _contextFactory.CreateDbContext();

            Task<List<PerspectiveDto>> perspectiveTask =
                context1.tbl_perspective
                    .Where(x => x.isactive)
                    .OrderBy(x => x.id)
                    .Select(x => new PerspectiveDto
                    {
                        id = x.id,
                        perspective = x.perspective
                    })
                    .AsNoTracking()
                    .ToListAsync();

            Task<List<StrategicObjectiveDto>> objectiveTask =
                context2.tbl_strategic_objective
                    .Where(x => x.isactive)
                    .OrderBy(x => x.id)
                    .Select(x => new StrategicObjectiveDto
                    {
                        id = x.id,
                        objective = x.strategicobjective
                    })
                    .AsNoTracking()
                    .ToListAsync();

            Task<List<KpiDetailsDto>> kpiTask =
                context3.tbl_kpi_master
                    .Where(x => x.isactive)
                    .OrderBy(x => x.id)
                    .Select(x => new KpiDetailsDto
                    {
                        id = x.id,
                        kpiname = x.kpiname
                    })
                    .AsNoTracking()
                    .ToListAsync();

            await Task.WhenAll(perspectiveTask, objectiveTask, kpiTask);

            CreateTemplateDropdownDto response = new CreateTemplateDropdownDto
            {
                perspectiveList = perspectiveTask.Result,
                strategicObjectiveList = objectiveTask.Result,
                kpiDetailsDtos = kpiTask.Result
            };

            return response;

        }

        public async Task<ResultDto> GetBscTemplateById(SelectPerspectiveKpiDto bsc)
        {
            FinancialYearRange fy = GetFinancialYearRange(DateTime.UtcNow);

            DateTime fyStart = fy.start;
            DateTime fyEnd = fy.end;


            BscFormHeader? exists = await _db.tbl_bsc_form_header

                // Perspective + Perspective Master
                .Include(x => x.tbl_bsc_perspective)
                    .ThenInclude(p => p.tbl_perspective)

                // Perspective -> Objective -> Objective Master
                .Include(x => x.tbl_bsc_perspective)
                    .ThenInclude(p => p.tbl_bsc_strategic_objective)
                        .ThenInclude(o => o.tbl_strategic_objective)

                // Perspective -> Objective -> KPI -> KPI Master
                .Include(x => x.tbl_bsc_perspective)
                    .ThenInclude(p => p.tbl_bsc_strategic_objective)
                        .ThenInclude(o => o.tbl_bsc_kpi)
                            .ThenInclude(k => k.tbl_kpi_master)

                .AsNoTracking().FirstOrDefaultAsync(x =>
                    x.createdon >= fyStart &&
                    x.createdon <= fyEnd &&
                    (bsc.departmentId == 0 || x.departmentid == bsc.departmentId) &&
                    (bsc.stationId == 0 || x.stationid == bsc.stationId)
                );



            if (exists == null)
            {

                return new ResultDto
                {
                    status = "Failure",
                    message = "No records found."
                };
            }

            // STEP 1 — Fill header details into the incoming DTO
            bsc.formId = exists.id;

            // STEP 2 — Prepare list for perspectives
            bsc.perspective = new List<PerspectiveObjectiveSelected>();

            // STEP 3 — Map children (Perspective → Objective → KPI)
            foreach (BscPerspective p in exists.tbl_bsc_perspective)
            {
                PerspectiveObjectiveSelected pDto = new PerspectiveObjectiveSelected();
                pDto.perspectiveId = p.perspectiveid;
                pDto.perspectiveName = p?.tbl_perspective?.perspective;
                // Each perspective has only one objective in your system
                BscStrategicObjective? obj = p.tbl_bsc_strategic_objective.FirstOrDefault();

                if (obj != null)
                {
                    pDto.strategicObjectiveId = obj.strategicobjectiveid;
                    pDto.strategicObjectiveName = obj?.tbl_strategic_objective?.strategicobjective;
                    pDto.kpiList = new List<KpiSelectedDto>();

                    foreach (BscKpi k in obj.tbl_bsc_kpi)
                    {
                        KpiSelectedDto kdto = new KpiSelectedDto();
                        kdto.kpiId = k.kpiid;
                        kdto.frequency = k.frequency;
                        kdto.kpiName = k.tbl_kpi_master.kpiname;
                        pDto.kpiList.Add(kdto);
                    }
                }
                else
                {
                    // Safe default structure
                    pDto.kpiList = new List<KpiSelectedDto>();
                }

                bsc.perspective.Add(pDto);
            }

            return new ResultDto
            {
                status = "Success",
                formData = bsc
            };


        }
        public async Task<ResultDto> SaveBscTemplate(SelectPerspectiveKpiDto bsc)
        {
            BscFormHeader? exists = await GetBscTemplateForFyAsync(bsc);


            if (exists == null)
            {
                return await CreateNewTemplate(bsc);
            }

            if (exists.issharedbyadmin)
            {
                return new ResultDto
                {
                    status = "Failure",
                    message = "Template already exists for selected department for this FY."
                };
            }

            UpdateTemplate(exists, bsc);
            await _db.SaveChangesAsync();

            return new ResultDto
            {
                status = "Success",
                message = "Template updated fully."
            };
        }

        private async Task<ResultDto> CreateNewTemplate(SelectPerspectiveKpiDto bsc)
        {
            int submittedId = await _db.tbl_object_master
                .Where(x => x.value == "Draft - Template Creation")
                .Select(x => x.id)
                .FirstAsync();

            var form = new BscFormHeader
            {
                departmentid = bsc.departmentId,
                stationid = bsc.stationId,
                statusid = submittedId,
                createdby = "System",
                createdon = DateTime.UtcNow,
                updatedby = "System",
                updatedon = DateTime.UtcNow,
                isactive = true,
                tbl_bsc_perspective = new List<BscPerspective>()
            };

            foreach (var p in bsc.perspective)
            {
                var perspective = new BscPerspective
                {
                    perspectiveid = p.perspectiveId,
                    createdby = "System",
                    createdon = DateTime.UtcNow,
                    updatedby = "System",
                    updatedon = DateTime.UtcNow,
                    tbl_bsc_strategic_objective = new List<BscStrategicObjective>()
                };

                // Create objective
                var objective = new BscStrategicObjective
                {
                    strategicobjectiveid = p.strategicObjectiveId,
                    createdby = "System",
                    createdon = DateTime.UtcNow,
                    updatedby = "System",
                    updatedon = DateTime.UtcNow,
                    tbl_bsc_kpi = new List<BscKpi>()
                };

                objective.tbl_bsc_form_header = form;
                perspective.tbl_bsc_strategic_objective.Add(objective);

                // Add KPIs
                foreach (var k in p.kpiList)
                {
                    var kpi = new BscKpi
                    {
                        kpiid = k.kpiId,
                        frequency = k.frequency,
                        createdby = "System",
                        createdon = DateTime.UtcNow,
                        updatedby = "System",
                        updatedon = DateTime.UtcNow,
                        tbl_bsc_form_header = form
                    };

                    objective.tbl_bsc_kpi.Add(kpi);
                }

                // Add perspective into form
                form.tbl_bsc_perspective.Add(perspective);
            }

            _db.tbl_bsc_form_header.Add(form);
            await _db.SaveChangesAsync();

            bsc.formId = form.id;

            return new ResultDto
            {
                status = "Success"
            };
        }

        private void UpdateTemplate(BscFormHeader exists, SelectPerspectiveKpiDto bsc)
        {
            exists.updatedby = "System";
            exists.updatedon = DateTime.UtcNow;

            // STEP 1 — Update header data
            if (bsc.departmentId != 0)
            {
                exists.departmentid = bsc.departmentId;
            }


            if (bsc.stationId != 0)
            {
                exists.stationid = bsc.stationId;
            }


            // Convert existing items to dictionaries for O(1) lookup
            var existingPerspDict = exists.tbl_bsc_perspective
                .ToDictionary(x => x.perspectiveid, x => x);

            var incomingPerspIds = bsc.perspective
                .Select(x => x.perspectiveId)
                .ToHashSet();

            // STEP 2 — Add / Update Perspectives
            foreach (var incomingPersp in bsc.perspective)
            {
                if (!existingPerspDict.TryGetValue(incomingPersp.perspectiveId, out var persp))
                {
                    persp = new BscPerspective
                    {
                        bscformid = exists.id,
                        perspectiveid = incomingPersp.perspectiveId,
                        createdby = "System",
                        createdon = DateTime.UtcNow,
                        updatedby = "System",
                        updatedon = DateTime.UtcNow,
                        tbl_bsc_strategic_objective = new List<BscStrategicObjective>()
                    };
                    exists.tbl_bsc_perspective.Add(persp);
                }
                else
                {
                    persp.updatedon = DateTime.UtcNow;
                    persp.updatedby = "System";
                }

                // Handle Objective (One per Perspective)
                var existingObj = persp.tbl_bsc_strategic_objective
                    .FirstOrDefault(x => x.strategicobjectiveid == incomingPersp.strategicObjectiveId);

                if (existingObj == null)
                {
                    existingObj = new BscStrategicObjective
                    {
                        bscformid = exists.id,
                        bscperspectiveid = persp.id,
                        strategicobjectiveid = incomingPersp.strategicObjectiveId,
                        createdby = "System",
                        createdon = DateTime.UtcNow,
                        updatedby = "System",
                        updatedon = DateTime.UtcNow,
                        tbl_bsc_kpi = new List<BscKpi>()
                    };
                    persp.tbl_bsc_strategic_objective.Add(existingObj);
                }
                else
                {
                    existingObj.updatedon = DateTime.UtcNow;
                    existingObj.updatedby = "System";
                }

                // Convert existing KPIs to dictionary
                var existingKpiDict = existingObj.tbl_bsc_kpi
                    .ToDictionary(k => k.kpiid, k => k);

                // Add / Update KPIs
                foreach (var incomingKpi in incomingPersp.kpiList)
                {
                    if (!existingKpiDict.TryGetValue(incomingKpi.kpiId, out var kpi))
                    {
                        existingObj.tbl_bsc_kpi.Add(new BscKpi
                        {
                            kpiid = incomingKpi.kpiId,
                            bscformid = exists.id,
                            frequency = incomingKpi.frequency,
                            createdby = "System",
                            createdon = DateTime.UtcNow,
                            updatedby = "System",
                            updatedon = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        kpi.frequency = incomingKpi.frequency;
                        kpi.updatedon = DateTime.UtcNow;
                        kpi.updatedby = "System";
                    }
                }

                // STEP 3 — Remove deleted KPIs
                var incomingKpiIds = incomingPersp.kpiList.Select(x => x.kpiId).ToHashSet();
                var deletedKpis = existingObj.tbl_bsc_kpi
                    .Where(k => !incomingKpiIds.Contains(k.kpiid))
                    .ToList();

                foreach (var del in deletedKpis)
                {
                    existingObj.tbl_bsc_kpi.Remove(del);
                }
            }

            // STEP 4 — Remove deleted Perspectives (clean-up)
            var deletedPerspectives = exists.tbl_bsc_perspective
                .Where(p => !incomingPerspIds.Contains(p.perspectiveid))
                .ToList();

            foreach (var del in deletedPerspectives)
            {
                exists.tbl_bsc_perspective.Remove(del);
            }
        }


        private FinancialYearRange GetFinancialYearRange(DateTime date)
        {
            int fyYear = date.Month >= 4 ? date.Year : date.Year - 1;

            FinancialYearRange range = new FinancialYearRange();
            range.start = DateTime.SpecifyKind(new DateTime(fyYear, 4, 1), DateTimeKind.Utc);
            range.end = DateTime.SpecifyKind(new DateTime(fyYear + 1, 3, 31, 23, 59, 59), DateTimeKind.Utc);

            return range;

        }

        private async Task<BscFormHeader?> GetBscTemplateForFyAsync(SelectPerspectiveKpiDto bsc)
        {
            FinancialYearRange fy = GetFinancialYearRange(DateTime.UtcNow);

            DateTime fyStart = fy.start;
            DateTime fyEnd = fy.end;

            return await _db.tbl_bsc_form_header
                .Include(x => x.tbl_bsc_perspective)
                    .ThenInclude(p => p.tbl_bsc_strategic_objective)
                        .ThenInclude(o => o.tbl_bsc_kpi)
                .FirstOrDefaultAsync(x =>
                    x.createdon >= fyStart &&
                    x.createdon <= fyEnd &&
                    (bsc.departmentId == 0 || x.departmentid == bsc.departmentId) &&
                    (bsc.stationId == 0 || x.stationid == bsc.stationId)
                );
        }
    }
}
