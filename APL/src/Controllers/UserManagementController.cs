using APL.Entities;
using APL.Models;
using APL.Services;
using APL.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace APL.Controllers
{
    [Route("api/[controller]/[action]")]
    //[Authorize(Roles = "Admin")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {

        private readonly IUserManagementService _service;

        public UserManagementController(IUserManagementService service)
        {
            _service = service;
        }

        // GET: api/UserManagement/GetAllUsers
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {

            UserDepartmentDto result = await _service.GetAllUsers();

            var dto = new CustomResponse<UserDepartmentDto>
            {
                status = "success",
                data = result
            };

            return Ok(dto);


        }

        // https://localhost:7056/UserManagement/CreateUser
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserCreateDto user)
        {
            int result = await _service.CreateUser(user);

            CustomResponse<int> dto = new CustomResponse<int>
            {
                status = "success",
                data = result
            };

            return Ok(dto);
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteUser(UserCreateDto user)
        {

            bool result = await _service.DeleteUser(user);

            CustomResponse<bool> dto = new CustomResponse<bool>
            {
                status = result ? "Success" : "Failure",
                data = result
            };

            return Ok(dto);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserCreateDto user)
        {

            bool result = await _service.UpdateUser(user);

            CustomResponse<bool> dto = new CustomResponse<bool>
            {
                status = result ? "Success" : "Failure",
                data = result
            };

            return Ok(dto);
        }



    }
}
