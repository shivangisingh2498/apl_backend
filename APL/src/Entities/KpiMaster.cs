using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APL.Entities
{
    [Table("tbl_kpi_master")]
    public class KpiMaster
    {
        [Key]
        public int id { get; set; }
        public string? kpiname { get; set; }
        public string? definition { get; set; }
        public string? formula { get; set; }
        public string? uom { get; set; }
        public bool isbetter { get; set; } 
        public bool isactive { get; set; } = true;
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public ICollection<BscKpi> tbl_bsc_kpi { get; set; } = new List<BscKpi>();
    }

}
