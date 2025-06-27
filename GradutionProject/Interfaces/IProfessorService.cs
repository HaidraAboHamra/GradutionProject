using GradutionProject.Controllers;

namespace GradutionProject.Interfaces
{
    public interface IProfessorService
    {
        Task<IEnumerable<ProfessorDto>> GetAllAsync();
        Task<ProfessorDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateProfessorDto input);
        Task<bool> UpdateAsync(int id, UpdateProfessorDto input);
        Task<bool> DeleteAsync(int id);
    }

}
