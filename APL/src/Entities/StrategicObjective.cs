using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APL.Entities
{
    [Table("tbl_strategic_objective")]
    public class StrategicObjective
    {
        [Key]
        public int id { get; set; }
        public string? strategicobjective { get; set; }
        public bool isactive { get; set; } = true;
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public ICollection<BscStrategicObjective> tbl_strategic_objective { get; set; } = new List<BscStrategicObjective>();
    }

}
