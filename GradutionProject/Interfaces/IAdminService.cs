using GradutionProject.Abstractions;
using GradutionProject.Controllers;
using static GradutionProject.Controllers.AdminsController;

namespace GradutionProject.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<AdminDto>> GetAllAsync();
        Task<AdminDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateAdminDto input);
        Task<bool> UpdateAsync(int id, UpdateAdminDto input);
        Task<Result> ChangePasswordAsync(int userId, string role, string newPassword);
        Task<bool> DeleteAsync(int id);
    }

}
