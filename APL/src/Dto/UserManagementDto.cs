using APL.Entities;
using System.ComponentModel.DataAnnotations;

namespace APL.Models
{
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
        public string? supervisor { get; set; }
        public string? type { get; set; }
        public bool isActive { get; set; } = true;
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
    }
}
