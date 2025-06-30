using GradutionProject.Data;
using GradutionProject.Entities;
using GradutionProject.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GradutionProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ApplicationDbContext _context;

        public AdminsController(IAdminService adminService, ApplicationDbContext context)
        {
            _adminService = adminService;
            _context = context;
        }

        // GET: api/admins
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var admins = await _adminService.GetAllAsync();
            return Ok(admins);
        }

        // GET: api/admins/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AdminDto>> Get(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin == null)
                return NotFound(new { message = $"Admin with ID {id} not found." });
            return Ok(admin);
        }

        // POST: api/admins
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateAdminDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Admins.AnyAsync(a => a.Email == dto.Email))
                return Conflict(new { message = "Email is already used." });

            var id = await _adminService.CreateAsync(dto);

            return CreatedAtAction(nameof(Get), new { id }, dto);
        }

        // PUT: api/admins/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateAdminDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingAdmin = await _adminService.GetByIdAsync(id);
            if (existingAdmin == null)
                return NotFound(new { message = $"Admin with ID {id} not found." });

            if (existingAdmin.Email != dto.Email &&
                await _context.Admins.AnyAsync(a => a.Email == dto.Email && a.Id != id))
                return Conflict(new { message = "Email is already used by another admin." });

            var updated = await _adminService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound(new { message = $"Admin with ID {id} not found." });

            return NoContent();
        }

        // DELETE: api/admins/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _adminService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Admin with ID {id} not found." });
            return NoContent();
        }
        // PUT: api/admins/changepassword
        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordByAdminDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.ChangePasswordAsync(dto.UserId, dto.Role, dto.NewPassword);

            if (result.IsFailure)
                return BadRequest(new { message = result.Error?.ToString() ?? "Failed to change password." });

            return NoContent();
        }
    }

    public class AdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        // لا ترجع كلمة السر في DTO للردود
        public string Phone { get; set; }
        public string? CollegeName { get; set; }
    }

    public class UpdateAdminDto
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string? Password { get; set; }

        [Phone]
        public string Phone { get; set; }
    }

    public class CreateAdminDto
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Phone]
        public string Phone { get; set; }
    }
    public class ChangePasswordByAdminDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [RegularExpression("^(student|professor|admin)$", ErrorMessage = "Role must be 'student', 'professor', or 'admin'.")]
        public string Role { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string NewPassword { get; set; } = string.Empty;
    }


}
