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

        public BscTemplateService(AplDbContext db)
        {
            _db = db;
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
    }
}
