using APL.Entities;
using APL.Models;

namespace APL.Services
{
    public interface IMasterDataService
    {
        public Task<List<PerspectiveDto>> GetAllPerspective();
        public Task<long> CreatePerspective(PerspectiveDto dto);

        public Task<bool> DeletePerspective(DeleteIdDto dto);

        public Task<bool> UpdatePerspective(PerspectiveDto dto);
        public Task<long> CreateStrategicObjective(StrategicObjectiveDto dto);
        public Task<bool> DeleteStrategicObjective(DeleteIdDto dto);
        public Task<List<StrategicObjectiveDto>> GetAllStrategicObjective();
        public Task<bool> UpdateStrategicObjective(StrategicObjectiveDto dto);
        public Task<List<KpiDto>> GetKpiList();


    }
}
