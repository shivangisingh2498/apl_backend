using APL.Data;
using APL.Entities;
using APL.Models;
using APL.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
namespace APL.Services
{
    public class FormMasterService : IFormMasterService
    {
        private readonly AplDbContext _db;

        public FormMasterService(AplDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<FormMaster>> GetAllFormTypes()
        {
            var result = await _db.tbl_form_master
               .AsNoTracking()
               .OrderBy(f => f.id)
               .ToListAsync();

            return result;

        }
        public async Task<IEnumerable<PerspectiveDto>> GetPerspectiveData()
        {
            var result = await _db.tbl_perspective
               .AsNoTracking().
               Select(
                x => new PerspectiveDto
                {
                    id = x.id,
                    perspective = x.perspective,                    
                }
                )
               .OrderBy(f => f.id)
               .ToListAsync();

            return result;
        }

    }
}
