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

        // GET: api/BscTemplate/GetAllBU
        [HttpGet]
        public async Task<IActionResult> GetAllBU()
        {

            List<DepartmentMasterDto> result = await _service.GetBUList();

            var dto = new CustomResponse<List<DepartmentMasterDto>>
            {
                status = "success",
                data = result
            };

            return Ok(dto);


        }

        

    }
}
