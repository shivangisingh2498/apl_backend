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

        public async Task<UserDepartmentDto> GetAllUsers()
        {
            List<UserManagementDto>? userList = await _db.tbl_user_management.Include(x=>x.tbl_department_master).Include(x=>x.tbl_roles_master)
                .AsNoTracking().Where(x=>x.isactive).Select(x=> new UserManagementDto
                {
                    id = x.id,
                    name = x.name,
                    email = x.email,
                    departmentName = x.tbl_department_master.department,
                    supervisor = x.supervisor,
                    type = x.tbl_roles_master.roles
                })
               .AsNoTracking()
               .OrderByDescending(f => f.id)
               .ToListAsync();

            List<DepartmentMasterDto>? departmentList = await _db.tbl_department_master.AsNoTracking()
               .Where(x => x.isactive).Select(x => new DepartmentMasterDto
               {
                   id = x.id,
                   departmentName = x.department
               })
              .AsNoTracking()
              .OrderByDescending(f => f.id)
              .ToListAsync();

            List<RolesDto>? rolesList = await _db.tbl_roles_master.AsNoTracking()
               .Where(x => x.isactive).Select(x => new RolesDto
               {
                   id = x.id,
                   roles = x.roles
               })
              .AsNoTracking()
              .OrderByDescending(f => f.id)
              .ToListAsync();

            UserDepartmentDto result = new UserDepartmentDto
            {
                userList = userList,
                departmentList = departmentList,
                rolesList = rolesList
            };

            return result;

        }


        public async Task<int> CreateUser(UserCreateDto dto)
        {

            UserManagement entity = new UserManagement
            {
                name = dto?.name?.Trim(),
                email = dto?.email?.Trim(),
                departmentid = dto.departmentId,
                typeid = dto.typeId,
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
                entity.departmentid = dto.departmentId;
                entity.typeid = dto.typeId;
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
