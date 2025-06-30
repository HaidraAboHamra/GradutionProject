namespace GradutionProject.Services.AdminService
{
    using GradutionProject.Abstractions;
    using GradutionProject.Controllers;
    using GradutionProject.Data;
    using GradutionProject.Entities;
    using GradutionProject.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;

        public AdminService(ApplicationDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<IEnumerable<AdminDto>> GetAllAsync()
        {
            return await _context.Admins.Include(a => a.College).Select(a => new AdminDto
            {
                Id = a.Id,
                Name = a.Name,
                Email = a.Email,
                Phone = a.Phone,
                CollegeName = a.College.Name
            }).ToListAsync();
        }

        public async Task<AdminDto?> GetByIdAsync(int id)
        {
            var a = await _context.Admins.Include(a => a.College).FirstOrDefaultAsync(a => a.Id == id);
            if (a == null) return null;

            return new AdminDto
            {
                Id = a.Id,
                Name = a.Name,
                Email = a.Email,
                Phone = a.Phone,
                CollegeName = a.College?.Name
            };
        }

        public async Task<int> CreateAsync(CreateAdminDto input)
        {
            var admin = new Admin
            {
                Name = input.Name,
                Email = input.Email,
                Phone = input.Phone,
                Password = _passwordService.HashPassword(input.Password)
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return admin.Id;
        }

        public async Task<bool> UpdateAsync(int id, UpdateAdminDto input)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
                return false;

            admin.Name = input.Name;
            admin.Email = input.Email;
            admin.Phone = input.Phone;

            if (!string.IsNullOrWhiteSpace(input.Password))
                admin.Password = _passwordService.HashPassword(input.Password);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
                return false;

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            return true;
        }
    public async Task<Result> ChangePasswordAsync(int userId, string role, string newPassword)
        {
            try
            {
                switch (role.ToLower())
                {
                    case "student":
                        return await ChangeStudentPasswordAsync(userId, newPassword);

                    case "professor":
                        return await ChangeProfessorPasswordAsync(userId, newPassword);

                    case "admin":
                        return await ChangeAdminPasswordAsync(userId, newPassword);

                    default:
                        return Result.Failure(new Error("Invalid role specified."));
                }
            }
            catch (Exception ex)
            {
                // يمكن تسجيل الخطأ هنا باستخدام logger (إن وجد)
                return Result.Failure(new Error($"An error occurred: {ex.Message}"));
            }
        }

        private async Task<Result> ChangeStudentPasswordAsync(int userId, string newPassword)
        {
            var student = await _context.Students.FindAsync(userId);
            if (student == null)
                return Result.Failure(new Error("Student not found."));

            student.PasswordHash = _passwordService.HashPassword(newPassword);
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        private async Task<Result> ChangeProfessorPasswordAsync(int userId, string newPassword)
        {
            var professor = await _context.Professors.FindAsync(userId);
            if (professor == null)
                return Result.Failure(new Error("Professor not found."));

            professor.Password = _passwordService.HashPassword(newPassword);
            _context.Professors.Update(professor);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        private async Task<Result> ChangeAdminPasswordAsync(int userId, string newPassword)
        {
            var admin = await _context.Admins.FindAsync(userId);
            if (admin == null)
                return Result.Failure(new Error("Admin not found."));

            admin.Password = _passwordService.HashPassword(newPassword);
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

    }

}
