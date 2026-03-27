using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APL.Entities
{
    [Table("tbl_object_master")]
    public class ObjectMaster
    {
        [Key]
        public int id { get; set; }
        public string value { get; set; } = null!;
        public string description { get; set; } = null!;
        public bool isactive { get; set; } = true;
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public ICollection<BscAuditTrail> tbl_bsc_audit_trail { get; set; } = new List<BscAuditTrail>();
    }

}
