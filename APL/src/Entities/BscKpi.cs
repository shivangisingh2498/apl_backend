using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace APL.Entities
{
    [Table("tbl_bsc_kpi")]
    public class BscKpi
    {
        [Key]
        public int id { get; set; }
        public int bscformid { get; set; }
        public int bscstrategicobjectiveid { get; set; }
        public int kpiid { get; set; }
        public string? frequency { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public BscFormHeader? tbl_bsc_form_header { get; set; }
        public StrategicObjective? tbl_strategic_objective { get; set; }
        public KpiMaster? tbl_kpi_master { get; set; }
        public BscStrategicObjective? tbl_bsc_strategic_objective { get; set; }
    }

}
