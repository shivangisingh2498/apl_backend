
namespace APL.Models
{
    public class KpiDto
    {
        public int id { get; set; }
        public string? kpiname { get; set; }
        public string? definition { get; set; }
        public string? formula { get; set; }
        public string? uom { get; set; }
        public bool isbetter { get; set; }
    }
    public class KpiDetailsDto
    {
        public int id { get; set; }
        public string? kpiname { get; set; }
    }


}
