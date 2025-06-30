using GradutionProject.Controllers;

namespace GradutionProject.Interfaces;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllAsync(int collegeId, int page, int pageSize);
    Task<IEnumerable<StudentDto>> GetAllStudentAsync(int collegeId, int page, int pageSize);
    Task<StudentDto?> GetByIdAsync(int id);
    Task<List<StudentDto>> SearchByNameAsync(int collegeId, string name, int pageNumber, int pageSize);
    Task<int> RegisterAsync(StudentRegisterDto dto);
    Task<int> PromoteAsync(int newStudentId);
    Task<bool> UpdateAsync(int collegeId, int id, StudentRegisterDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteStudentAsync(int id);
}
