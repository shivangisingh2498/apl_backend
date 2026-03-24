using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace APL.Entities
{
    [Table("tbl_bsc_perspective")]
    public class BscPerspective
    {
        [Key]
        public int id { get; set; }
        public int bscformid { get; set; }
        public int perspectiveid { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public BscFormHeader? tbl_bsc_form_header { get; set; }
        public Perspective? tbl_perspective { get; set; }
        public ICollection<BscStrategicObjective>? tbl_bsc_strategic_objective { get; set; } = new List<BscStrategicObjective>();
    }

}
