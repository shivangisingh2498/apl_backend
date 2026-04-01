
namespace APL.Models
{


    public class TargetSettingsDto
    {
        public int id { get; set; }
        public string? kpiname { get; set; }
        public string? definition { get; set; }
        public string? formula { get; set; }
        public string? uom { get; set; }
        public bool isbetter { get; set; }
    }

    public class TargetTemplateDto
    {
        public int formId { get; set; }
        public int departmentId { get; set; }
        public string? department { get; set; }
        public int stationId { get; set; }
        public string? stationName { get; set; }
        public List<TargetPerspectiveDto> perspective { get; set; } = new();
    }
    public class TargetPerspectiveDto
    {
        public int perspectiveId { get; set; }
        public string? perspective { get; set; }
        public int strategicObjectiveId { get; set; }
        public string? strategicObjective { get; set; }
        public List<TargetKpiDto> kpiList { get; set; } = new();
    }
    public class MonthlyTargetDto
    {
        public int monthNo { get; set; }
        public decimal targetValue { get; set; }
    }

    public class TargetKpiDto
    {
        public int kpiId { get; set; }
        public string? frequency { get; set; }
        public string? definition { get; set; }
        public string? uom { get; set; }
        public string? minValue { get; set; }
        public string? maxValue { get; set; }
        public YearlyTargetDto? yearlyTarget { get; set; }
       
        public List<MonthlyTargetDto>? monthlyTarget { get; set; }
    }
    public class YearlyTargetDto
    {
        public decimal targetCurrentYear { get; set; }
        public decimal targetNextYear { get; set; }
        public decimal targetNextToNextYear { get; set; }
        public decimal weightage { get; set; }
        public string? initiative { get; set; }
    }
    public class FrequencyRuleResponseDto
    {
        public List<FrequencyRuleDto> frequencyRule { get; set; } = new();
        public YearlyHeaderDto? yearlyHeaderDto { get; set; }

    }

    public class FrequencyRuleDto
    {
        public string frequency { get; set; } = null!;
        public List<MonthDto> months { get; set; } = new();
    }

    public class MonthDto
    {
        public int id { get; set; }          // month number (1–12)
        public string value { get; set; } = null!;
    }

    public class TargetYearDto
    {
        public int year { get; set; }
    }
    public class FrequencyMappingDto
    {
        public int freqencyId { get; set; }
        public string? frequency { get; set; }
    }

    public class FrequencyMonthRule
    {
        public int frequencyId { get; set; }
        public string frequencyName { get; set; } = string.Empty;
        public int monthNo { get; set; }
    }
   public class YearlyHeaderDto
    {
        public string CurrentYearTargetHeader { get; set; } = string.Empty;
        public string NextYearTargetHeader { get; set; } = string.Empty;
        public string NextToNextYearTargetHeader { get; set; } = string.Empty;
    }

}
