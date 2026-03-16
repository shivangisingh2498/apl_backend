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
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class FormController : ControllerBase
    {

        private readonly IFormMasterService _service;
        private readonly ILogger<FormController> _logger;

        public FormController(IFormMasterService service, ILogger<FormController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/Form/GetAllFormTypes
        [HttpGet]
        public async Task<IActionResult> GetAllFormTypes()
        {
            try
            {
                var result = await _service.GetAllFormTypes();

                var dto = new CustomResponse<IEnumerable<FormMaster>>
                {
                    status = "success",
                    data = result 
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                var dto = new
                {
                    status = "error",
                    response = ex.Message
                };
                return BadRequest(dto);

            }

        }

        // GET: api/Form/GetPerspectiveObjective
        //based on form type selected get the perspective & objective
        [HttpGet]
        public async Task<IEnumerable<PerspectiveDto>> GetPerspectiveObjective()
        {            
            var result = await _service.GetPerspectiveData();
            return result;
        }

        // GET api/<FormController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FormController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<FormController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FormController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
