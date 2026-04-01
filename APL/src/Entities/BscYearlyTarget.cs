using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace APL.Entities
{
    [Table("tbl_bsc_yearly_target")]
    public class BscYearlyTarget
    {
        [Key]
        public int id { get; set; }
        public int bscformid { get; set; }
        public int kpiid { get; set; }
        public decimal pyactual { get; set; }
        public decimal currentyeartarget { get; set; }
        public decimal nextyeartarget { get; set; }
        public decimal nexttonextyeartarget { get; set; }
        public decimal weigthage { get; set; }
        public string? initiative { get; set; }
        public bool isactive { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public BscFormHeader? tbl_bsc_form_header { get; set; }
        public BscKpi? tbl_bsc_kpi { get; set; }

    }

}
