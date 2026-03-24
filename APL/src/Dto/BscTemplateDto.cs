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
        public int stationId { get; set; }
        public List<PerspectiveObjectiveSelected>? perspective{get; set;}
        
    }
    public class PerspectiveObjectiveSelected
    {
        public int bscPerspectiveId { get; set; }
        public int bscStrategicObjectiveId { get; set; }
        public int perspectiveId { get; set; }
        public int strategicObjectiveId { get; set; }
        public List<KpiSelectedDto>? kpiList { get; set; }
    }
    public class KpiSelectedDto
    {
        public int kid { get; set; }
        public int kpiId { get; set; }
        public string? frequency { get; set; }
        public bool isSharedByAdmin { get; set; }
        public int statusId { get; set; }
        public bool isSubmittedBySpoc { get; set; }
    }


}
