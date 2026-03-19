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
             .AsNoTracking().Where(x => x.isactive && x.station!="Default").Select(x => new StationDto
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
    }
}
