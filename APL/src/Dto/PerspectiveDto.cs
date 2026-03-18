using System.ComponentModel.DataAnnotations;

namespace APL.Models
{
    public class PerspectiveDto
    {
        public long id { get; set; }
        [Required]
        public string perspective { get; set; }

    }
    public class StrategicObjectiveDto
    {
        public long id { get; set; }
        public string? objective { get; set; }

    }
}
