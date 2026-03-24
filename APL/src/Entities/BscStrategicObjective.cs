using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace APL.Entities
{
    [Table("tbl_bsc_strategic_objective")]
    public class BscStrategicObjective
    {
        [Key]
        public int id { get; set; }
        public int bscformid { get; set; }
        public int strategicobjectiveid { get; set; }
        public int bscperspectiveid { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public BscFormHeader? tbl_bsc_form_header { get; set; }
        public StrategicObjective? tbl_strategic_objective { get; set; }
        public BscPerspective? tbl_bsc_perspective { get; set; }
        public ICollection<BscKpi>? tbl_bsc_kpi { get; set; }
    }

}
