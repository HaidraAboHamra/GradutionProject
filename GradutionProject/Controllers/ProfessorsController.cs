using GradutionProject.Data;
using GradutionProject.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GradutionProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfessorsController> _logger;

        public ProfessorsController(ApplicationDbContext context, ILogger<ProfessorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Professors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfessorDto>>> GetAll()
        {
            var professors = await _context.Professors
                .Include(p => p.College)
                .Select(p => new ProfessorDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Phone = p.Phone,
                    Email = p.Email,
                    CollegeId = p.CollegeId
                })
                .ToListAsync();

            return Ok(professors);
        }

        // GET: api/Professors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfessorDto>> Get(int id)
        {
            var professor = await _context.Professors
                .Include(p => p.College)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (professor == null)
                return NotFound(new { message = $"Professor with ID {id} not found." });

            var dto = new ProfessorDto
            {
                Id = professor.Id,
                Name = professor.Name,
                Phone = professor.Phone,
                Email = professor.Email,
                CollegeId = professor.CollegeId
            };

            return Ok(dto);
        }

        // POST: api/Professors
        [HttpPost]
        public async Task<ActionResult<ProfessorDto>> Create(CreateProfessorDto input)
        {
            if (await _context.Professors.AnyAsync(p => p.Email == input.Email))
                return Conflict(new { message = "Email already exists." });

            var professor = new Professor
            {
                Name = input.Name,
                Phone = input.Phone,
                Email = input.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password),
                CollegeId = input.CollegeId
            };

            _context.Professors.Add(professor);
            await _context.SaveChangesAsync();

            var dto = new ProfessorDto
            {
                Id = professor.Id,
                Name = professor.Name,
                Phone = professor.Phone,
                Email = professor.Email,
                CollegeId = professor.CollegeId
            };

            return CreatedAtAction(nameof(Get), new { id = professor.Id }, dto);
        }

        // PUT: api/Professors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProfessorDto input)
        {
            var professor = await _context.Professors.FindAsync(id);
            if (professor == null)
                return NotFound(new { message = $"Professor with ID {id} not found." });

            professor.Name = input.Name;
            professor.Phone = input.Phone;
            professor.Email = input.Email;

            if (!string.IsNullOrWhiteSpace(input.Password))
                professor.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);

            professor.CollegeId = input.CollegeId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Professors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var professor = await _context.Professors.FindAsync(id);
            if (professor == null)
                return NotFound(new { message = $"Professor with ID {id} not found." });

            _context.Professors.Remove(professor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
    public class ProfessorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? CollegeId { get; set; }
    }

    public class CreateProfessorDto
    {
        [Required]
        public string Name { get; set; }

        [Phone]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public int? CollegeId { get; set; }
    }

    public class UpdateProfessorDto
    {
        [Required]
        public string Name { get; set; }

        [Phone]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public int? CollegeId { get; set; }
    }

}
