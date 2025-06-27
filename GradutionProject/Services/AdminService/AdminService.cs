namespace GradutionProject.Services.AdminService
{
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


    }

}
