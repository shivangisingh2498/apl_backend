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
                stationList = stationList,
                // frequencyList = freqList

            };
        }

        public async Task<CreateTemplateDropdownDto> GetBscTemplateDropdown()
        {


            AplDbContext context1 = _contextFactory.CreateDbContext();
            AplDbContext context2 = _contextFactory.CreateDbContext();
            AplDbContext context3 = _contextFactory.CreateDbContext();
            AplDbContext context4 = _contextFactory.CreateDbContext();

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

            List<FrequencyDto> freqList = await context4.tbl_object_master.Where(x => x.description == "Frequency")
.AsNoTracking().Select(x => new FrequencyDto
{
    id = x.id,
    frequnecy = x.value
})
.AsNoTracking()
.OrderBy(f => f.id)
.ToListAsync();

            await Task.WhenAll(perspectiveTask, objectiveTask, kpiTask);

            CreateTemplateDropdownDto response = new CreateTemplateDropdownDto
            {
                perspectiveList = perspectiveTask.Result,
                strategicObjectiveList = objectiveTask.Result,
                kpiDetailsDtos = kpiTask.Result,
                frequencyList = freqList 
                
            };

            return response;

        }

        public async Task<ResultDto<SelectPerspectiveKpiDto>> GetBscTemplateById(SelectPerspectiveKpiDto bsc)
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
                // Perspective -> Objective -> KPI -> KPI Master
                .Include(x => x.tbl_bsc_perspective)
                    .ThenInclude(p => p.tbl_bsc_strategic_objective)
                        .ThenInclude(o => o.tbl_bsc_kpi)
                            .ThenInclude(k => k.tbl_kpi_master)
                 .Include(x => x.tbl_bsc_perspective)
                    .ThenInclude(p => p.tbl_bsc_strategic_objective)
                        .ThenInclude(o => o.tbl_bsc_kpi)
                            .ThenInclude(k => k.tbl_object_master)

                .AsNoTracking().FirstOrDefaultAsync(x =>
                    x.createdon >= fyStart &&
                    x.createdon <= fyEnd &&
                    (bsc.departmentId == 0 || x.departmentid == bsc.departmentId) &&
                    (bsc.stationId == 0 || x.stationid == bsc.stationId)
                );



            if (exists == null)
            {

                return new ResultDto<SelectPerspectiveKpiDto>
                {
                    status = "Failure",
                    message = "No records found.",
                    result = null
                };
            }

            // STEP 1 — Fill header details into the incoming DTO
            bsc.formId = exists.id;

            // STEP 2 — Prepare list for perspectives
            bsc.perspective = new List<PerspectiveObjectiveSelected>();

            // STEP 3 — Map children (Perspective → Objective → KPI)
            foreach (BscPerspective p in exists.tbl_bsc_perspective.OrderBy(x => x.id))
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
                        kdto.frequencyId = k.tbl_object_master.id;
                        kdto.frequency = k?.tbl_object_master?.value;
                        kdto.kpiName = k?.tbl_kpi_master?.kpiname;
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

            return new ResultDto<SelectPerspectiveKpiDto>
            {
                status = "Success",
                result = bsc
            };


        }
        public async Task<ResultDto<SelectPerspectiveKpiDto>> SaveBscTemplate(SelectPerspectiveKpiDto bsc)
        {
            BscFormHeader? exists = await GetBscTemplateForFyAsync(bsc);


            if (exists == null)
            {
                return await CreateNewTemplate(bsc);
            }

            if (exists.issharedbyadmin)
            {
                return new ResultDto<SelectPerspectiveKpiDto>
                {
                    status = "Failure",
                    message = "Template already exists for selected department for this FY.",
                    result = null
                };
            }

            UpdateTemplate(exists, bsc);
            await _db.SaveChangesAsync();

            return new ResultDto<SelectPerspectiveKpiDto>
            {
                status = "Success",
                message = "Template updated fully.",
                result = bsc
            };
        }

        public async Task<ResultDto<List<UserManagementDto>>> GetSpocDetails(SelectPerspectiveKpiDto bsc)
        {
            BscFormHeader? exists = await GetBscTemplateForFyAsync(bsc);

            List<UserManagementDto>? spocList = await GetSpoc(bsc);
            if (spocList == null || spocList.Count == 0)
            {
                return new ResultDto<List<UserManagementDto>>
                {
                    status = "Failure",
                    message = "Share failed: No active SPOC mapped to this Department",
                    result = null
                };
            }
            return new ResultDto<List<UserManagementDto>>
            {
                status = "Success",
                result = spocList
            }; ;

        }

        public async Task<ResultDto<string>> ShareBscTemplate(SelectPerspectiveKpiDto bsc)
        {
            //change the status in header
            BscFormHeader? form = await _db.tbl_bsc_form_header.FirstOrDefaultAsync(x =>
                    x.id == bsc.formId);
            if (form != null)
            {
                form.issharedbyadmin = true;
                form.templatecreatedate = DateTime.UtcNow;
            }
            else
            {
                return new ResultDto<string>
                {
                    status = "Failure",
                    message = "No remplate found"
                };
            }


            //get the spoc details
            var spocList = await GetSpoc(bsc);

            if (spocList == null || spocList.Count == 0)
            {
                return new ResultDto<string>
                {
                    status = "Failure",
                    message = "Share failed: No active SPOC mapped to this Department."
                };
            }


            int submittedId = await _db.tbl_object_master
               .Where(x => x.value == "Form Created")
               .Select(x => x.id)
               .FirstAsync();



            foreach (UserManagementDto user in spocList)
            {
                BscAuditTrail audit = new BscAuditTrail
                {
                    userid = user.id,
                    formstatusid = submittedId,
                    createdby = "System",
                    createdon = DateTime.UtcNow,
                    updatedby = "System",
                    updatedon = DateTime.UtcNow,
                    isactive = true,
                };
                form.tbl_bsc_audit_trail.Add(audit);
            }

            await _db.SaveChangesAsync();

            bsc.formId = form.id;

            return new ResultDto<string>
            {
                status = "Success"
            };

            //imsert in the formheader
        }
        private async Task<ResultDto<SelectPerspectiveKpiDto>> CreateNewTemplate(SelectPerspectiveKpiDto bsc)
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
                        frequencyid = k.frequencyId,
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

            return new ResultDto<SelectPerspectiveKpiDto>
            {
                status = "Success",
                result = bsc
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
                            frequencyid = incomingKpi.frequencyId,
                            createdby = "System",
                            createdon = DateTime.UtcNow,
                            updatedby = "System",
                            updatedon = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        kpi.frequencyid = incomingKpi.frequencyId;
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
                    (bsc.department == "Default" || x.departmentid == bsc.departmentId) &&
                    (bsc.stationName == "Default" || x.stationid == bsc.stationId)
                );
        }

        private async Task<List<UserManagementDto>?> GetSpoc(SelectPerspectiveKpiDto bsc)
        {
            List<UserManagementDto>? spocList = await _db.tbl_user_management.Include(x => x.tbl_department_master).Include(x => x.tbl_station_master).Include(x => x.tbl_roles_master)
                .AsNoTracking().Where(x => x.isactive &&
                  (bsc.department == "Default" || x.departmentid == bsc.departmentId) &&
                  (bsc.stationName == "Default" || x.stationid == bsc.stationId)

                ).Select(x => new UserManagementDto
                {
                    id = x.id,
                    name = x.name,
                    email = x.email,
                    departmentName = x.tbl_department_master.department,
                    stationName = x.tbl_station_master.station == "Default" ? "" : x.tbl_station_master.station,
                    supervisor = x.supervisor,
                    type = x.tbl_roles_master.roles
                })
                .Where(x => x.type == "SPOC")
               .AsNoTracking()
               .OrderByDescending(f => f.id)
               .ToListAsync();
            return spocList;
        }
    }
}
