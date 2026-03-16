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
            var result = await _db.tbl_perspective.Include(x => x.tbl_strategic_objective)
               .AsNoTracking().
               Select(
                x => new PerspectiveDto
                {
                    id = x.id,
                    perspective = x.perspective,
                    strategicObjective = x.tbl_strategic_objective.Select(x => new ObjectiveDto
                    {
                        id = x.id,
                        objective = x.strategicobjective
                    }).ToList()
                }
                )
               .OrderBy(f => f.id)
               .ToListAsync();

            return result;
        }

    }
}
