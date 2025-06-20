using GradutionProject.Data;
using GradutionProject.Entities;
using GradutionProject.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradutionProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<Admin> _passwordHasher;
    public AdminsController(ApplicationDbContext context,IPasswordHasher<Admin> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    // GET: api/Admins
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAdmins()
    {
        var admins = await _context.Admins
            .Include(a => a.College)
            .ToListAsync();

        var result = admins.Select(a => new
        {
            a.Id,
            a.Name,
            a.Phone,
            a.Email,
            CollegeName = a.College != null ? a.College.Name : null
        });

        return Ok(result);
    }


    // GET: api/Admins/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetAdmin(int id)
    {
        var admin = await _context.Admins
            .Include(a => a.College)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (admin == null)
        {
            return NotFound(new { message = $"Admin with ID {id} not found." });
        }

        var result = new
        {
            admin.Id,
            admin.Name,
            admin.Phone,
            admin.Email,
            CollegeName = admin.College != null ? admin.College.Name : null
        };

        return Ok(result);
    }


    // POST: api/Admins
    [HttpPost]
    public async Task<ActionResult> PostAdmin(AdminDto dto)
    {
        var admin = new Admin
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password,
            Phone = dto.Phone,
        };
        admin.Password = _passwordHasher.HashPassword(admin, admin.Password);
        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();

        return Ok(dto);
    }

    // PUT: api/Admins/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAdmin(int id, Admin admin)
    {
        if (id != admin.Id)
        {
            return BadRequest();
        }

        _context.Entry(admin).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AdminExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Admins/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdmin(int id)
    {
        var admin = await _context.Admins.FindAsync(id);
        if (admin == null)
        {
            return NotFound();
        }

        _context.Admins.Remove(admin);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AdminExists(int id)
    {
        return _context.Admins.Any(e => e.Id == id);
    }
    public class AdminDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }

    }
}
