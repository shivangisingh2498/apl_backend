using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APL.Entities
{
    [Table("tbl_user_management")]
    public class UserManagement
    {
        [Key] //--standard 2350 * 4, delux 2750
        public int id { get; set; }
        public string? name { get; set; } 
        public string? email { get; set; } 
        public int departmentid { get; set; }
        public int stationid { get; set; }
        public string? supervisor { get; set; }
        public int typeid { get; set; }
        public bool isactive { get; set; }
        public bool isdisable { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public RolesMaster? tbl_roles_master { get; set; }
        public DepartmentMaster? tbl_department_master { get; set; }
        public StationMaster? tbl_station_master { get; set; }
    }

}
