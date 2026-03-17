using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APL.Entities
{
    public class StationMaster
    {
        [Key]
        public int id { get; set; }
        public string? station { get; set; }
        public bool isactive { get; set; } = true;
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        [JsonIgnore]
        public ICollection<UserManagement> tbl_user_management { get; set; } = new List<UserManagement>();
    }
}
