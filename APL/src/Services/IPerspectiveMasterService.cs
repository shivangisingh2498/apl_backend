using APL.Entities;
using APL.Models;

namespace APL.Services
{
    public interface IPerspectiveMasterService
    {
        Task<IEnumerable<UserManagementDto>> GetAllPerspective();
        public Task<int> CreatePerspective(UserCreateDto dto);

        public Task<bool> DeletePerspective(UserCreateDto dto);

        public Task<bool> UpdatePerspective(UserCreateDto dto);
    }
}
