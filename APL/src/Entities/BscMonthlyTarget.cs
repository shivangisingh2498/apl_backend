using APL.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace APL.Entities
{
    [Table("tbl_bsc_monthly_target")]
    public class BscMonthlyTarget
    {
        [Key]
        public int id { get; set; }
        public int bscformid { get; set; }
        public int kpiid { get; set; }
        public int monthno { get; set; }
        public int year { get; set; }
        public decimal targetvalue { get; set; }
        public bool isactive { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public BscFormHeader? tbl_bsc_form_header { get; set; }
        public BscKpi? tbl_bsc_kpi { get; set; }

    }

}
