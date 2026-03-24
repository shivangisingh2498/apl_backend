using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APL.Entities
{
    [Table("tbl_station_master")]
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
        public ICollection<BscFormHeader> tbl_bsc_form_header { get; set; } = new List<BscFormHeader>();
    }
}
