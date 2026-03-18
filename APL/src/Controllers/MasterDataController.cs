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
    public class MasterDataController : ControllerBase
    {

        private readonly IMasterDataService _service;

        public MasterDataController(IMasterDataService service)
        {
            _service = service;
        }

        // GET: api/MasterData/GetPerspectiive
        [HttpGet]
        public async Task<IActionResult> GetPerspective()
        {

            List<PerspectiveDto> result = await _service.GetAllPerspective();

            var dto = new CustomResponse<List<PerspectiveDto>>
            {
                status = "success",
                data = result
            };

            return Ok(dto);


        }

        [HttpPost]
        public async Task<IActionResult> CreatePerspective(PerspectiveDto perspective)
        {
            long result = await _service.CreatePerspective(perspective);

            CustomResponse<long> dto = new CustomResponse<long>
            {
                status = "success",
                data = result
            };

            return Ok(dto);
        }


        [HttpDelete]
        public async Task<IActionResult> DeletePerspective(PerspectiveDto perspective)
        {

            bool result = await _service.DeletePerspective(perspective);

            CustomResponse<bool> dto = new CustomResponse<bool>
            {
                status = result ? "Success" : "Failure",
                data = result
            };

            return Ok(dto);
        }


        [HttpPost]
        public async Task<IActionResult> UpdatePerspective(PerspectiveDto perspective)
        {

            bool result = await _service.UpdatePerspective(perspective);

            CustomResponse<bool> dto = new CustomResponse<bool>
            {
                status = result ? "Success" : "Failure",
                data = result
            };

            return Ok(dto);
        }


        [HttpGet]
        public async Task<IActionResult> GetStrategicObjective()
        {

            List<StrategicObjectiveDto> result = await _service.GetAllStrategicObjective();

            var dto = new CustomResponse<List<StrategicObjectiveDto>>
            {
                status = "success",
                data = result
            };

            return Ok(dto);


        }

        [HttpPost]
        public async Task<IActionResult> CreateStrategicObjective(StrategicObjectiveDto obj)
        {
            long result = await _service.CreateStrategicObjective(obj);

            CustomResponse<long> dto = new CustomResponse<long>
            {
                status = "success",
                data = result
            };

            return Ok(dto);
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteStrategicObjective(StrategicObjectiveDto obj)
        {

            bool result = await _service.DeleteStrategicObjective(obj);

            CustomResponse<bool> dto = new CustomResponse<bool>
            {
                status = result ? "Success" : "Failure",
                data = result
            };

            return Ok(dto);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStrategicObjective(StrategicObjectiveDto obj)
        {

            bool result = await _service.UpdateStrategicObjective(obj);

            CustomResponse<bool> dto = new CustomResponse<bool>
            {
                status = result ? "Success" : "Failure",
                data = result
            };

            return Ok(dto);
        }


        [HttpGet]
        public async Task<IActionResult> GetKpi()
        {

            List<KpiDto> result = await _service.GetKpiList();

            var dto = new CustomResponse<List<KpiDto>>
            {
                status = "success",
                data = result
            };

            return Ok(dto);


        }


    }
}
