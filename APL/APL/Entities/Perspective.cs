using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APL.Entities
{
    [Table("tbl_perspective")]
    public class Perspective
    {
        [Key]
        public long id { get; set; }
        public string perspective { get; set; } = null!;
        public bool isactive { get; set; } = true;
        public string? createdby { get; set; }
        public DateTimeOffset? createddttm { get; set; }
        public string? updatedby { get; set; }
        public DateTimeOffset? updateddttm { get; set; }
        [JsonIgnore]
        public ICollection<StrategicObjective> tbl_strategic_objective{ get; set; } = new List<StrategicObjective>();
    }

}
