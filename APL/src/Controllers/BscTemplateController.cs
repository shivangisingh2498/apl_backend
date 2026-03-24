using APL.Entities;
using APL.Models;
using APL.Services;
using APL.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APL.Controllers
{
    [Route("api/[controller]/[action]")]
    //[Authorize(Roles = "Admin")]
    [ApiController]
    public class BscTemplateController : ControllerBase
    {

        private readonly IBscTemplateService _service;

        public BscTemplateController(IBscTemplateService service)
        {
            _service = service;
        }

        // GET: api/BscTemplate/GetBscList
        [HttpGet]
        public async Task<IActionResult> GetBscList()
        {

            StationDepartmentDto result = await _service.GetBscList();

            var dto = new CustomResponse<StationDepartmentDto>
            {
                status = "Success",
                data = result
            };

            return Ok(dto);


        }

        [HttpGet]
        public async Task<IActionResult> GetBscTemplateDropdown()
        {

            CreateTemplateDropdownDto result = await _service.GetBscTemplateDropdown();

            var dto = new CustomResponse<CreateTemplateDropdownDto>
            {
                status = "Success",
                data = result
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SaveBscTemplate(SelectPerspectiveKpiDto bsc)
        {

            ResultDto result = await _service.SaveBscTemplate(bsc);

            var dto = new CustomResponse<ResultDto>
            {
                status = "Success",
                data = result
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> GetBscTemplateById(SelectPerspectiveKpiDto bsc)
        {

            ResultDto result = await _service.GetBscTemplateById(bsc);

            var dto = new CustomResponse<ResultDto>
            {
                status = "Success",
                data = result
            };

            return Ok(dto);
        }



    }
}
