namespace APL.Models
{
    public class BscTemplateDto
    {
        public int departmentid { get; set; }
        public int stationid { get; set; }
    }
    public class SelectPerspectiveKpiDto
    {
        public int formId { get; set; }
        public int departmentId { get; set; }
        public string? department { get; set; }
        public int stationId { get; set; }
        public string? stationName { get; set; }
        public List<PerspectiveObjectiveSelected>? perspective{get; set;}
        
    }
    public class PerspectiveObjectiveSelected
    {
        public int perspectiveId { get; set; }
        public string? perspectiveName { get; set; }
        public int strategicObjectiveId { get; set; }
        public string? strategicObjectiveName { get; set; }
        public List<KpiSelectedDto>? kpiList { get; set; }
    }
    public class KpiSelectedDto
    {
        public int kpiId { get; set; }
        public string? kpiName { get; set; }
        public string? frequency { get; set; }
        public int frequencyId { get; set; }
    }

    public class FinancialYearRange
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }


}
