using APL.Entities;
using APL.Models;
using APL.Services;
using APL.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace APL.Controllers
{
    [Route("api/[controller]/[action]")]
    //[Authorize(Roles = "SPOC")]
    [ApiController]
    public class TargetSettingsController : ControllerBase
    {

        private readonly ITargetSettingsService _service;

        public TargetSettingsController(ITargetSettingsService service)
        {
            _service = service;
        }

        // Post: api/TargetSettings/GetTemplateByRole
        [HttpGet]
        public async Task<IActionResult> GetTemplateByRole()
        {
            string? name = CommonHelperMethods.GetPreferredUsername(User);

            if (!string.IsNullOrEmpty(name))
            {
                List<TargetTemplateDto> result = await _service.GetTemplateByRole(name);
                var dto = new CustomResponse<List<TargetTemplateDto>>
                {
                    status = "Success",
                    data = result
                };
                return Ok(dto);
            }


            else
            {
                return Unauthorized();
            }                
        }

        [HttpPost]

        public async Task<IActionResult> GetFrequencyRules(TargetYearDto target)
        {
            var result = await _service.GetFrequencyRulesAsync(target.year);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTarget([FromBody] TargetTemplateDto target)
        {
            var result = await _service.SaveTemplateTarget(target);
            return Ok(result);
        }

    }
}
