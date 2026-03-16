using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APL.Entities
{
    [Table("tbl_strategic_objective")]
    public class StrategicObjective
    {
        [Key]
        public long id { get; set; }
        public long perspectiveid { get; set; }
        public string? strategicobjective { get; set; }
        public bool isactive { get; set; } = true;
        public string? createdby { get; set; }
        public DateTimeOffset? createddttm { get; set; }
        public string? updatedby { get; set; }
        public DateTimeOffset? updateddttm { get; set; }
        public Perspective? tbl_perspective { get; set; }
    }

}
