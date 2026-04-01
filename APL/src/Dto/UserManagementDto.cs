using APL.Entities;
using System.ComponentModel.DataAnnotations;

namespace APL.Models
{
    public class StationDepartmentDto
    {
        public List<DepartmentMasterDto>? departmentList { get; set; }
        public List<StationDto>? stationList { get; set; }
      
    }
    public class UserDepartmentDto
    {
        public List<UserManagementDto>?  userList { get; set; }
        public List<DepartmentMasterDto>? departmentList { get; set; }
        public List<RolesDto>? rolesList { get; set; }
        public List<StationDto>? stationList { get; set; }
    }

    public class UserManagementDto
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? departmentName { get; set; }
        public string? stationName { get; set; }
        public string? supervisor { get; set; }
        public string? type { get; set; }
        public bool isActive { get; set; } = true;
        public bool isDisable { get; set; } = true;
    }
    public class UserDepartmentRoleDto
    {
        public int id { get; set; }
        public int departmentId { get; set; }
        public int roleId { get; set; }
        public string? role { get; set; }
    }
    public class UserCreateDto
    {
        public int id { get; set; }
        [Required]
        public string? name { get; set; }
        [Required]
        public string? email { get; set; }
        [Required]
        public int departmentId { get; set; }
        [Required]
        public string? supervisor { get; set; }
        [Required]
        public int stationid { get; set; }
        [Required]
        public int typeId { get; set; }
    }
    public class DeleteIdDto
    {
        public int id { get; set; }
    }
    public class DisableUserDto
    {
        public int id { get; set; }
        public bool isDisable { get; set; }
    }
    public class DepartmentMasterDto
    {
        public int id { get; set; }
        public string? department { get; set; }
        public string? departmentName { get; set; }
        public bool isActive { get; set; } = true;
    }

    public class RolesDto
    {
        public int id { get; set; }
        public string? roles { get; set; }
        public string? description { get; set; }
    }

    public class StationDto
    {
        public int id { get; set; }
        public string? station { get; set; }
        public bool isActive { get; set; }
    }


}
