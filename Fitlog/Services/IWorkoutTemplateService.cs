using Progress.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Progress.Services
{
    public interface IWorkoutTemplateService
    {
        Task<List<WorkoutTemplateDTO>> GetAllAsync(string userId);
        Task<WorkoutTemplateDTO> GetByIdAsync(int id, string userId);
        Task<int> CreateAsync(WorkoutTemplateDTO dto, string userId);
        Task<bool> UpdateAsync(int id, WorkoutTemplateDTO dto, string userId);
        Task<bool> DeleteAsync(int id, string userId);

        // NOVÉ: kontrola, zda byla template použita
        Task<bool> TemplateHasFinishedWorkoutAsync(int id, string userId);
    }
}
