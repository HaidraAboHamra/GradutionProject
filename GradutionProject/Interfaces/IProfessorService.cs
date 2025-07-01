using GradutionProject.Controllers;

namespace GradutionProject.Interfaces
{
    public interface IProfessorService
    {
        Task<(IEnumerable<ProfessorDto> data, int totalPages)> GetAllAsync(int collegeId,int page, int pageSize);
        Task<(IEnumerable<ProfessorDto> data, int totalPages)> GetAllAsync(int page, int pageSize);
        Task<ProfessorDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(int collegeId, CreateProfessorDto input);
        Task<bool> UpdateAsync(int id, UpdateProfessorDto input);
        Task<bool> DeleteAsync(int id);
    }

}
