namespace APL.Models
{
    public class PerspectiveDto
    {
        public long id { get; set; }
        public string? perspective { get; set; } 
        public List<ObjectiveDto>? strategicObjective { get; set; }

    }
    public class ObjectiveDto
    {
        public long id { get; set; }
        public string? objective { get; set; }

    }
}
