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

        public async Task<List<DepartmentMasterDto>> GetBUList()
        {
            List<DepartmentMasterDto> result = await _db.tbl_department_master
                .AsNoTracking().Where(x=>x.isactive).Select(x=> new DepartmentMasterDto
                {
                    id = x.id,
                    department = x.department,
                    departmentName = x.departmentname                   
                })
               .AsNoTracking()
               .OrderBy(f => f.id)
               .ToListAsync();

            return result;
        }

    }
}
