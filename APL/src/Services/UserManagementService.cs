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
            List<UserManagementDto>? userList = await _db.tbl_user_management.Include(x=>x.tbl_department_master).Include(x=>x.tbl_station_master).Include(x=>x.tbl_roles_master)
                .AsNoTracking().Where(x=>x.isactive).Select(x=> new UserManagementDto
                {
                    id = x.id,
                    name = x.name,
                    email = x.email,
                    departmentName = x.tbl_department_master.department,
                    stationName = x.tbl_station_master.station == "Default"? "" : x.tbl_station_master.station,
                    supervisor = x.supervisor,
                    type = x.tbl_roles_master.roles,
                    isDisable  = x.isdisable
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

            List<StationDto>? stationList = await _db.tbl_station_master.AsNoTracking()
               .Where(x => x.isactive).Select(x => new StationDto
               {
                   id = x.id,
                   station = x.station
               })
              .AsNoTracking()
              .OrderByDescending(f => f.id)
              .ToListAsync();

            UserDepartmentDto result = new UserDepartmentDto
            {
                userList = userList,
                departmentList = departmentList,
                rolesList = rolesList,
                stationList = stationList
            };

            return result;

        }


        public async Task<int> CreateUser(UserCreateDto dto)
        {
            //check user with same email and department exists

            List<UserManagementDto>? userList = await _db.tbl_user_management.Include(x => x.tbl_department_master).Include(x => x.tbl_roles_master)
                .AsNoTracking().Where(x => x.isactive && x.departmentid == dto.departmentId
                && x.typeid == dto.typeId && x.email == dto.email).Select(x => new UserManagementDto
                {
                    id = x.id,
                    name = x.name,
                    email = x.email,
                    departmentName = x.tbl_department_master.department,
                    stationName = x.tbl_station_master.station == "Default" ? "" : x.tbl_station_master.station,
                    supervisor = x.supervisor,
                    type = x.tbl_roles_master.roles
                })
               .OrderByDescending(f => f.id)
               .ToListAsync();

            if (userList.Count > 0 )
            {
                return 0;
            }
            UserManagement entity = new UserManagement
            {
                name = dto?.name?.Trim(),
                email = dto?.email?.Trim(),
                departmentid = dto.departmentId,
                typeid = dto.typeId,
                stationid = dto.stationid,
                supervisor = string.IsNullOrWhiteSpace(dto.supervisor) ? null : dto.supervisor.Trim(),
                isactive = true,
                isdisable = false,
                createdon = DateTime.UtcNow,
                updatedon = DateTime.UtcNow,
                createdby = "System", //get eid from token 
                updatedby = "System" //get eid from token 
            };

            _db.tbl_user_management.Add(entity);
            await _db.SaveChangesAsync();
            return entity.id;
        }

        public async Task<bool> DeleteUser(DeleteIdDto dto)
        {
            UserManagement? entity = await _db.tbl_user_management
                                  .FirstOrDefaultAsync(x => x.id == dto.id);

            if(entity != null)
            {
                entity.isactive = false;
                entity.isdisable = true;
                await _db.SaveChangesAsync();
                return true;
            }      
            return false;
        }

        public async Task<bool> DisableEnableUser(DisableUserDto dto)
        {
            UserManagement? entity = await _db.tbl_user_management
                                  .FirstOrDefaultAsync(x => x.id == dto.id);

            if (entity != null)
            {
                entity.isdisable = dto.isDisable;
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
