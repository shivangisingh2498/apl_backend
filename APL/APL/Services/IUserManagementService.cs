using APL.Entities;
using APL.Models;

namespace APL.Services
{
    public interface IUserManagementService
    {
        Task<IEnumerable<UserManagementDto>> GetAllUsers();
        public Task<int> CreateUser(UserCreateDto dto);

        public Task<bool> DeleteUser(UserCreateDto dto);

        public Task<bool> UpdateUser(UserCreateDto dto);
    }
}
