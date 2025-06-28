using GradutionProject.Data;
using GradutionProject.Entities;
using GradutionProject.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradutionProject.Controllers;

using GradutionProject.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api/[controller]")]
public class AdminsController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ApplicationDbContext _context;

    public AdminsController(IAdminService adminService,ApplicationDbContext context)
    {
        _adminService = adminService;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var result = await _adminService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> Get(int id)
    {
        var result = await _adminService.GetByIdAsync(id);
        if (result is null)
            return NotFound(new { message = $"Admin with ID {id} not found." });
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateAdminDto dto)
    {
        var id = await _adminService.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateAdminDto dto)
    {
        var updated = await _adminService.UpdateAsync(id, dto);
        if (!updated)
            return NotFound(new { message = $"Admin with ID {id} not found." });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _adminService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Admin with ID {id} not found." });
        return NoContent();
    }
    private int? GetClaimValue(string claimType)
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == claimType);
        if (claim != null && int.TryParse(claim.Value, out int value))
            return value;
        return null;
    }
    private bool AdminExists(int id)
    {
        return _context.Admins.Any(e => e.Id == id);
    }
}


    public class AdminDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public int Id { get; internal set; }
        public string? CollegeName { get; internal set; }
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
