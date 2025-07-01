using GradutionProject.Data;
using GradutionProject.Entities;
using GradutionProject.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GradutionProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorsController : ControllerBase
    {
        private readonly IProfessorService _professorService;
        private readonly ILogger<ProfessorsController> _logger;

        public ProfessorsController(IProfessorService professorService, ILogger<ProfessorsController> logger)
        {
            _professorService = professorService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
          
            var (data, totalPages) = await _professorService.GetAllAsync(page, pageSize);
            return Ok(new { data, totalPages });
        }
        [HttpGet("/admin")]
        public async Task<ActionResult> GetAllPrfoessorByAdmin([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var collegeId = GetClaimValue("CollegeId");
            if (collegeId is null)
                return Forbid();
            var (data, totalPages) = await _professorService.GetAllAsync((int)collegeId, page, pageSize);
            return Ok(new { data, totalPages });
        }



        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var result = await _professorService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Professor with ID {id} not found." });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateProfessorDto dto)
        {
            var collegeId = GetClaimValue("CollegeId");

            if (collegeId == null)
                return Unauthorized(new { message = "Invalid token claims." });

            var id = await _professorService.CreateAsync((int)collegeId, dto);
            return CreatedAtAction(nameof(Get), new { id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateProfessorDto dto)
        {
            var updated = await _professorService.UpdateAsync(id, dto);
            if (!updated)
                return NotFound(new { message = $"Professor with ID {id} not found." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _professorService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Professor with ID {id} not found." });
            return NoContent();
        }
        private int? GetClaimValue(string claimType)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == claimType);
            if (claim != null && int.TryParse(claim.Value, out int value))
                return value;
            return null;
        }
    }
    public class ProfessorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? CollegeId { get; set; }
        public string? CollegeName { get; internal set; }
        public string? CoursName { get; internal set; }
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
