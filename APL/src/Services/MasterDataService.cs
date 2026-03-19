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
    public class MasterDataService : IMasterDataService
    {
        private readonly AplDbContext _db;

        //-- perspective functions
        public MasterDataService(AplDbContext db)
        {
            _db = db;
        }

        public async Task<long> CreatePerspective(PerspectiveDto dto)
        {
            Perspective entity = new Perspective
            {
                perspective = dto.perspective,
                isactive = true,
                createdon = DateTime.UtcNow,
                updatedon = DateTime.UtcNow,
                createdby = "System", //get eid from token 
                updatedby = "System" //get eid from token 
            };

            _db.tbl_perspective.Add(entity);
            await _db.SaveChangesAsync();
            return entity.id;
        }

        public  async Task<bool> DeletePerspective(PerspectiveDto dto)
        {
            Perspective? entity = await _db.tbl_perspective
                                  .FirstOrDefaultAsync(x => x.id == dto.id);

            if (entity != null)
            {
                _db.tbl_perspective.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }


            return false;
        }

        public async Task<List<PerspectiveDto>> GetAllPerspective()
        {
            List<PerspectiveDto> result = await _db.tbl_perspective
               .AsNoTracking().Where(x => x.isactive).Select(x => new PerspectiveDto
               {
                   id = x.id,
                   perspective = x.perspective
               })
              .AsNoTracking()
              .OrderByDescending(f => f.id)
              .ToListAsync();

            return result;
        }

        public async Task<bool> UpdatePerspective(PerspectiveDto dto)
        {
            Perspective? entity = await _db.tbl_perspective
                                .FirstOrDefaultAsync(x => x.id == dto.id);

            if (entity != null)
            {
                entity.perspective = dto.perspective;
               
                entity.updatedon = DateTime.UtcNow;
                entity.updatedby = "System"; //get eid from token 

                await _db.SaveChangesAsync();
                return true;
            }
            return false;

        }

        //--strategic objective functions

        public async Task<long> CreateStrategicObjective(StrategicObjectiveDto dto)
        {
            StrategicObjective entity = new StrategicObjective
            {
                strategicobjective = dto.objective,
                isactive = true,
                createdon = DateTime.UtcNow,
                updatedon = DateTime.UtcNow,
                createdby = "System", //get eid from token 
                updatedby = "System" //get eid from token 
            };

            _db.tbl_strategic_objective.Add(entity);
            await _db.SaveChangesAsync();
            return entity.id;
        }

        public async Task<bool> DeleteStrategicObjective(StrategicObjectiveDto dto)
        {
            StrategicObjective? entity = await _db.tbl_strategic_objective
                                  .FirstOrDefaultAsync(x => x.id == dto.id);

            if (entity != null)
            {
                _db.tbl_strategic_objective.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }


            return false;
        }

        public async Task<List<StrategicObjectiveDto>> GetAllStrategicObjective()
        {
            List<StrategicObjectiveDto> result = await _db.tbl_strategic_objective
               .AsNoTracking().Where(x => x.isactive).Select(x => new StrategicObjectiveDto
               {
                   id = x.id,
                   objective = x.strategicobjective
               })
              .AsNoTracking()
              .OrderByDescending(f => f.id)
              .ToListAsync();

            return result;
        }

        public async Task<bool> UpdateStrategicObjective(StrategicObjectiveDto dto)
        {
            StrategicObjective? entity = await _db.tbl_strategic_objective
                                .FirstOrDefaultAsync(x => x.id == dto.id);

            if (entity != null)
            {
                entity.strategicobjective = dto.objective;

                entity.updatedon = DateTime.UtcNow;
                entity.updatedby = "System"; //get eid from token 

                await _db.SaveChangesAsync();
                return true;
            }
            return false;

        }

        public async Task<List<KpiDto>> GetKpiList()
        {
            List<KpiDto> kpiList = await _db.tbl_kpi_master.AsNoTracking().Where(x => x.isactive).Select(x =>
                new KpiDto
                {
                    id = x.id,
                    kpiname = x.kpiname,
                    definition = x.definition,
                    formula = x.formula,
                    uom = x.uom,
                    isbetter = x.isbetter
                }
                ).ToListAsync();


            return kpiList;
        }

    }
}
