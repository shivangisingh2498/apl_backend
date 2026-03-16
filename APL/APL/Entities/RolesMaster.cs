using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APL.Entities
{
    [Table("tbl_roles_master")]
    public class RolesMaster
    {
        [Key]
        public int id { get; set; }
        public string roles { get; set; } = null!;
        public string description { get; set; } = null!;
        public bool isactive { get; set; } = true;
        public string? createdby { get; set; }
        public DateTimeOffset? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTimeOffset? updatedon { get; set; }
        [JsonIgnore]
        public ICollection<UserManagement> tbl_user_management { get; set; } = new List<UserManagement>();
    }

}
