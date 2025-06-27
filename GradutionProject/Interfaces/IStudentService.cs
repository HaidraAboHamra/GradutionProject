using GradutionProject.Controllers;

namespace GradutionProject.Interfaces;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllAsync(int page, int pageSize);
    Task<IEnumerable<StudentDto>> GetAllStudentAsync(int page, int pageSize);
    Task<StudentDto?> GetByIdAsync(int id);
    Task<int> RegisterAsync(StudentRegisterDto dto);
    Task<int> PromoteAsync(int newStudentId);
    Task<bool> UpdateAsync(int id, StudentRegisterDto dto);
    Task<bool> DeleteAsync(int id);
}
