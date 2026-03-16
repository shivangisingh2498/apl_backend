using APL.Entities;
using APL.Models;

namespace APL.Services
{
    public interface IBscTemplateService
    {
        Task<List<DepartmentMasterDto>> GetBUList();
    }
}
