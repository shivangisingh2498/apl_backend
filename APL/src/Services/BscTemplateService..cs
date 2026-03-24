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
                .AsNoTracking().Where(x => x.isactive).Select(x => new DepartmentMasterDto
                {
                    id = x.id,
                    department = x.department,
                    departmentName = x.departmentname
                })
               .AsNoTracking()
               .OrderBy(f => f.id)
               .ToListAsync();

            List<StationDto> stationList = await _db.tbl_station_master
             .AsNoTracking().Where(x => x.isactive && x.station != "Default").Select(x => new StationDto
             {
                 id = x.id,
                 station = x.station,
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

        public async Task<ResultDto> SaveBscTemplate(SelectPerspectiveKpiDto bsc)
        {

            BscFormHeader? exists = await _db.tbl_bsc_form_header
                .Include(x => x.tbl_bsc_perspective)
                    .ThenInclude(p => p.tbl_bsc_strategic_objective)
                        .ThenInclude(o => o.tbl_bsc_kpi)
                .FirstOrDefaultAsync(x =>
                    x.createdon.HasValue &&
                    x.createdon.Value.Year == DateTime.UtcNow.Year &&
                    (x.departmentid == bsc.departmentId || x.stationid == bsc.stationId));


            if (exists == null)
            {
                int submittedId = _db.tbl_object_master.Where(x => x.value == "Draft - Template Creation").First().id;
                BscFormHeader form = new BscFormHeader
                {
                    departmentid = bsc.departmentId,
                    stationid = bsc.stationId,
                    issharedbyadmin = false,
                    statusid = submittedId, //-- set status from 
                    issubmittedbyspoc = false,
                    isactive = true,
                    createdby = "System", //get eid from token 
                    createdon = DateTime.UtcNow,
                    updatedby = "System",  //get eid from token 
                    updatedon = DateTime.UtcNow,
                    templatecreatedate = null,
                    spoctemplatesubmitdate = null
                };


                foreach (var p in bsc.perspective)
                {
                    // Create perspective
                    var perspective = new BscPerspective
                    {
                        perspectiveid = p.perspectiveId,
                        createdby = "System",
                        createdon = DateTime.UtcNow,
                        updatedby = "System",
                        updatedon = DateTime.UtcNow,
                        tbl_bsc_strategic_objective = new List<BscStrategicObjective>()
                    };

                    // Add Perspective to form 
                    form.tbl_bsc_perspective.Add(perspective);

                    // Create Objective
                    var objective = new BscStrategicObjective
                    {
                        strategicobjectiveid = p.strategicObjectiveId,
                        createdby = "System",
                        createdon = DateTime.UtcNow,
                        updatedby = "System",
                        updatedon = DateTime.UtcNow,

                        tbl_bsc_form_header = form,
                        tbl_bsc_perspective = perspective,

                        tbl_bsc_kpi = new List<BscKpi>()
                    };

                    // Add KPIs
                    foreach (var item in p.kpiList)
                    {
                        var kpi = new BscKpi
                        {
                            kpiid = item.kpiId,
                            frequency = item.frequency,
                            createdby = "System",
                            createdon = DateTime.UtcNow,
                            updatedby = "System",
                            updatedon = DateTime.UtcNow,
                            tbl_bsc_form_header = form,
                            tbl_bsc_strategic_objective = objective
                        };

                        objective.tbl_bsc_kpi.Add(kpi);
                    }

                    perspective.tbl_bsc_strategic_objective.Add(objective);
                }

                _db.tbl_bsc_form_header.Add(form);
                int save = await _db.SaveChangesAsync();

                bsc.formId = form.id;
                
                return new ResultDto
                {
                    status = "Success"
                };
            }
            else if (!exists.issharedbyadmin)
            {

                UpdateTemplate(exists, bsc);
                await _db.SaveChangesAsync();

                return new ResultDto
                {
                    status = "Success",
                    message = "Template updated successfully."
                };

            }
            else
            {
                return new ResultDto
                {
                    status = "Failure",
                    message = "Template already exists for selected department for this FY."
                };
            }

        }

        private void UpdateTemplate(BscFormHeader? exists, SelectPerspectiveKpiDto bsc)
        {
            //edit the template

            exists.updatedby = "System";
            exists.updatedon = DateTime.UtcNow;


            // STEP 1 — Update department & station fields
            exists.departmentid = bsc.departmentId;
            exists.stationid = bsc.stationId;

            foreach (var incomingPersp in bsc.perspective)
            {
                // Check if perspective already exists
                var existingPersp = exists.tbl_bsc_perspective
                    .FirstOrDefault(p => p.perspectiveid == incomingPersp.perspectiveId 
                    );

                if (existingPersp == null)
                {
                    // ADD NEW PERSPECTIVE
                    existingPersp = new BscPerspective
                    {
                        bscformid = exists.id,
                        perspectiveid = incomingPersp.perspectiveId,
                        createdon = DateTime.UtcNow,
                        createdby = "System",
                        updatedon = DateTime.UtcNow,
                        updatedby = "System",
                        tbl_bsc_strategic_objective = new List<BscStrategicObjective>()
                    };
                    exists.tbl_bsc_perspective.Add(existingPersp);
                }
                else
                {
                    // UPDATE EXISTING PERSPECTIVE
                    existingPersp.updatedon = DateTime.UtcNow;
                    existingPersp.updatedby = "System";
                }

                // STEP 3 — Handle strategic objective (1 per perspective)
                var existingObj = existingPersp.tbl_bsc_strategic_objective
                    .FirstOrDefault(o => o.strategicobjectiveid == incomingPersp.strategicObjectiveId);

                if (existingObj == null)
                {
                    // ADD NEW OBJECTIVE
                    existingObj = new BscStrategicObjective
                    {
                        bscformid = exists.id,
                        strategicobjectiveid = incomingPersp.strategicObjectiveId,
                        createdon = DateTime.UtcNow,
                        createdby = "System",
                        updatedon = DateTime.UtcNow,
                        updatedby = "System",
                        tbl_bsc_kpi = new List<BscKpi>()
                    };

                    existingPersp.tbl_bsc_strategic_objective.Add(existingObj);
                }
                else
                {
                    // UPDATE EXISTING OBJECTIVE
                    existingObj.updatedon = DateTime.UtcNow;
                    existingObj.updatedby = "System";
                }

                // STEP 4 — Sync KPI List inside the objective
                foreach (var incomingKpi in incomingPersp.kpiList)
                {
                    var existingKpi = existingObj.tbl_bsc_kpi
                        .FirstOrDefault(k => k.kpiid == incomingKpi.kpiId);

                    if (existingKpi == null)
                    {
                        // ADD NEW KPI
                        existingObj.tbl_bsc_kpi.Add(new BscKpi
                        {
                            kpiid = incomingKpi.kpiId,
                            bscformid = exists.id,
                            frequency = incomingKpi.frequency,
                            createdon = DateTime.UtcNow,
                            createdby = "System",
                            updatedon = DateTime.UtcNow,
                            updatedby = "System"
                        });
                    }
                    else
                    {
                        // UPDATE EXISTING KPI
                        existingKpi.frequency = incomingKpi.frequency;
                        existingKpi.updatedon = DateTime.UtcNow;
                        existingKpi.updatedby = "System";
                    }
                }
               

            }
           

        }
    }
}
