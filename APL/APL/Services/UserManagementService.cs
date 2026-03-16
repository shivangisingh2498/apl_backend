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
    public class UserManagementService : IUserManagementService
    {
        private readonly AplDbContext _db;

        public UserManagementService(AplDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<UserManagementDto>> GetAllUsers()
        {
            IEnumerable<UserManagementDto> result = await _db.tbl_user_management.Include(x=>x.tbl_department_master).Include(x=>x.tbl_object_master)
                .Where(x=>x.isactive).Select(x=> new UserManagementDto
                {
                    id = x.id,
                    name = x.name,
                    email = x.email,
                    departmentname = x.tbl_department_master.department,
                    supervisor = x.supervisor,
                    type = x.tbl_object_master.value
                })
               .AsNoTracking()
               .OrderByDescending(f => f.id)
               .ToListAsync();

            return result;

        }


        public async Task<int> CreateUser(UserCreateDto dto)
        {

            UserManagement entity = new UserManagement
            {
                name = dto?.name?.Trim(),
                email = dto?.email?.Trim(),
                departmentid = dto.departmentid,
                typeid = dto.typeid,
                supervisor = string.IsNullOrWhiteSpace(dto.supervisor) ? null : dto.supervisor.Trim(),
                isactive = true,
                createdon = DateTime.UtcNow,
                updatedon = DateTime.UtcNow,
                createdby = "System", //get eid from token 
                updatedby = "System" //get eid from token 
            };

            _db.tbl_user_management.Add(entity);
            await _db.SaveChangesAsync();
            return entity.id;
        }

        public async Task<bool> DeleteUser(UserCreateDto dto)
        {
            UserManagement? entity = await _db.tbl_user_management
                                  .FirstOrDefaultAsync(x => x.id == dto.id);

            if(entity != null)
            {
                _db.tbl_user_management.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
         

            return false;
        }


        public async Task<bool> UpdateUser(UserCreateDto dto)
        {
            UserManagement? entity = await _db.tbl_user_management
                                 .FirstOrDefaultAsync(x => x.id == dto.id);

            if(entity != null)
            {
                entity.name = dto?.name?.Trim();
                entity.email = dto?.email?.Trim();
                entity.departmentid = dto.departmentid;
                entity.typeid = dto.typeid;
                entity.supervisor = string.IsNullOrWhiteSpace(dto.supervisor) ? null : dto.supervisor.Trim();
                entity.isactive = true;
                entity.updatedon = DateTime.UtcNow;
                entity.updatedby = "System"; //get eid from token 
                //_db.Update(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;





        }


    }
}
