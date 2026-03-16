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
    public class UserManagementController : ControllerBase
    {

        private readonly IUserManagementService _service;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(IUserManagementService service, ILogger<UserManagementController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/UserManagement/GetAllUsers
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                UserDepartmentDto result = await _service.GetAllUsers();

                var dto = new CustomResponse<UserDepartmentDto>
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

        // https://localhost:7056/UserManagement/CreateUser
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserCreateDto user)
        {
            try
            {
                int result = await _service.CreateUser(user);

                CustomResponse<int> dto = new CustomResponse<int>
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


        [HttpDelete]
        public async Task<IActionResult> DeleteUser(UserCreateDto user)
        {
            try
            {
                bool result = await _service.DeleteUser(user);

                CustomResponse<bool> dto = new CustomResponse<bool>
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


        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserCreateDto user)
        {
            try
            {
                bool result = await _service.UpdateUser(user);

                CustomResponse<bool> dto = new CustomResponse<bool>
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



    }
}
