using APL.Entities;
using APL.Models;

namespace APL.Services
{
    public interface IFormMasterService
    {
        Task<IEnumerable<FormMaster>> GetAllFormTypes();

        Task<IEnumerable<PerspectiveDto>> GetPerspectiveData();
    }
}
