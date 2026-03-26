using APL.Entities;
using APL.Models;
using APL.Shared;

namespace APL.Services
{
    public interface IBscTemplateService
    {
        Task<StationDepartmentDto> GetBscList();
        Task<CreateTemplateDropdownDto> GetBscTemplateDropdown();
        Task<ResultDto<SelectPerspectiveKpiDto>> SaveBscTemplate(SelectPerspectiveKpiDto bsc);
        Task<ResultDto<SelectPerspectiveKpiDto>> GetBscTemplateById(SelectPerspectiveKpiDto bsc);
        Task<ResultDto<List<UserManagementDto>>> GetSpocDetails(SelectPerspectiveKpiDto bsc);
    }
}
