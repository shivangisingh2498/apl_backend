using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APL.Entities
{
    [Table("tbl_uom_master")]
    public class UomMaster
    {
        [Key]
        public int id { get; set; }
        public string? uom { get; set; }
        public decimal? minvalue { get; set; }
        public decimal? maxvalue { get; set; }
        public bool isbetter { get; set; } 
        public bool isactive { get; set; } = true;
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public ICollection<KpiMaster> tbl_kpi_master { get; set; } = new List<KpiMaster>();
    }

}
