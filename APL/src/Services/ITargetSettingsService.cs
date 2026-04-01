using APL.Entities;
using APL.Models;
using APL.Shared;

namespace APL.Services
{
    public interface ITargetSettingsService
    {

        public Task<List<TargetTemplateDto>> GetTemplateByRole(string input);

        public Task<FrequencyRuleResponseDto> GetFrequencyRulesAsync(int fyStartYear);

        public Task<ResultDto<string>> SaveTemplateTarget(TargetTemplateDto targetSetting);
    }
}
