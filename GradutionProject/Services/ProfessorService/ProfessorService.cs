namespace GradutionProject.Services.ProfessorService
{
    using GradutionProject.Controllers;
    using GradutionProject.Data;
    using GradutionProject.Entities;
    using GradutionProject.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class ProfessorService : IProfessorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;

        public ProfessorService(ApplicationDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<(IEnumerable<ProfessorDto> data, int totalPages)> GetAllAsync(int collegeId ,int page, int pageSize)
        {
            var totalCount = await _context.Professors.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var professors = await _context.Professors
                .Include(p => p.College)
                .Include(p => p.Cours)
                .Where(x=>x.CollegeId == collegeId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProfessorDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Email = p.Email,
                    Phone = p.Phone,
                    CollegeId = p.CollegeId,
                    CollegeName = p.College.Name,
                    CoursName = p.Cours.Name
                })
                .ToListAsync();

            return (professors, totalPages);
        }


        public async Task<ProfessorDto?> GetByIdAsync(int id)
        {
            var p = await _context.Professors.Include(p => p.College).Include(p => p.Cours).FirstOrDefaultAsync(p => p.Id == id);
            if (p == null) return null;

            return new ProfessorDto
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                Phone = p.Phone,
                CollegeId = p.CollegeId,
                CollegeName = p.College?.Name,
                CoursName = p.Cours.Name
            };
        }

        public async Task<int> CreateAsync(int collegeId,CreateProfessorDto input)
        {
            var professor = new Professor
            {
                Name = input.Name,
                Email = input.Email,
                Phone = input.Phone,
                CollegeId = collegeId,
                Password = _passwordService.HashPassword(input.Password)
            };
            _context.Professors.Add(professor);
            await _context.SaveChangesAsync();
            return professor.Id;
        }

        public async Task<bool> UpdateAsync(int id, UpdateProfessorDto input)
        {
            var professor = await _context.Professors.FindAsync(id);
            if (professor == null)
                return false;

            professor.Name = input.Name;
            professor.Email = input.Email;
            professor.Phone = input.Phone;
            professor.CollegeId = input.CollegeId;

            if (!string.IsNullOrWhiteSpace(input.Password))
                professor.Password = _passwordService.HashPassword(input.Password);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var professor = await _context.Professors.FindAsync(id);
            if (professor == null)
                return false;

            _context.Professors.Remove(professor);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
