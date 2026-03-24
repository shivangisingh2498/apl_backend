using APL.Entities;
using APL.Models;
using APL.Shared;

namespace APL.Services
{
    public interface IBscTemplateService
    {
        Task<StationDepartmentDto> GetBscList();
        Task<CreateTemplateDropdownDto> GetBscTemplateDropdown();
        Task<ResultDto> SaveBscTemplate(SelectPerspectiveKpiDto bsc);
    }
}
