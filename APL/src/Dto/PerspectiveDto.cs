using System.ComponentModel.DataAnnotations;

namespace APL.Models
{
    public class PerspectiveDto
    {
        public long id { get; set; }
        [Required]
        public string? perspective { get; set; }

    }
    public class StrategicObjectiveDto
    {
        public long id { get; set; }
        [Required]
        public string? objective { get; set; }

    }

    public class CreateTemplateDropdownDto
    {
        public List<PerspectiveDto>? perspectiveList { get; set; }
        public List<StrategicObjectiveDto>? strategicObjectiveList { get; set; }
        public List<KpiDetailsDto>? kpiDetailsDtos { get; set; }
        public List<FrequencyDto>? frequencyList { get; set; }
    }
    public class FrequencyDto
    {
        public int id { get; set; }
        public string? frequnecy { get; set; }
    }
}
