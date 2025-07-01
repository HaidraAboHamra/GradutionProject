using GradutionProject.Data;
using GradutionProject.Entities;
using GradutionProject.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GradutionProject.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CollegesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CollegesController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var colleges = await _context.Colleges
            .Include(c => c.Students)
            .Include(c => c.Professsors)
            .Select(c => new CollegeResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                YearOfStudy = c.YearOfStudy,
                AdminName = c.Admin.Name,
                StudentCount = c.Students.Count,
                ProfessorCount = c.Professsors.Count
            }).ToListAsync();

        return Ok(colleges);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var college = await _context.Colleges
            .Include(c => c.Students)
            .Include(c => c.Professsors)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (college == null)
            return NotFound();

        var dto = new CollegeResponseDto
        {
            Id = college.Id,
            Name = college.Name,
            Description = college.Description,
            AdminName = college.Admin.Name,
            YearOfStudy = college.YearOfStudy,
            StudentCount = college.Students.Count,
            ProfessorCount = college.Professsors.Count
        };

        return Ok(dto);
    }

    [Authorize]
    [HttpGet("mycollege")]
    public async Task<IActionResult> GetMyCollege()
    {
        var collegeId = User.Claims.FirstOrDefault(c => c.Type == "CollegeId")?.Value;

        if (string.IsNullOrEmpty(collegeId) || !int.TryParse(collegeId, out var id))
            return Unauthorized(new { message = "CollegeId not found in token." });

        var college = await _context.Colleges
            .Include(c => c.Students)
            .Include(c => c.Professsors)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (college == null)
            return NotFound("College not found.");

        var dto = new CollegeResponseDto
        {
            Id = college.Id,
            Name = college.Name,
            Description = college.Description,
            YearOfStudy = college.YearOfStudy,
            StudentCount = college.Students.Count,
            ProfessorCount = college.Professsors.Count
        };

        return Ok(dto);
    }


    [HttpPost]
    public async Task<IActionResult> Create(CollegeDto dto)
    {
        var college = new College
        {
            Name=dto.Name,
            Description=dto.Description,
            YearOfStudy=dto.YearOfStudy,
            AdminId=dto.AdminId,
        };
        _context.Colleges.Add(college);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = college.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, College updatedCollege)
    {
        var college = await _context.Colleges.FindAsync(id);
        if (!string.IsNullOrWhiteSpace(updatedCollege.Name) && college.Name != updatedCollege.Name)
            college.Name = updatedCollege.Name;

        if (college.YearOfStudy != updatedCollege.YearOfStudy)
            college.YearOfStudy = updatedCollege.YearOfStudy;

        if (updatedCollege.Description != null && college.Description != updatedCollege.Description)
            college.Description = updatedCollege.Description;
        
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var college = await _context.Colleges.FindAsync(id);
        if (college == null) return NotFound();
        _context.Colleges.Remove(college);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    private int? GetClaimValue(string claimType)
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == claimType);
        if (claim != null && int.TryParse(claim.Value, out int value))
            return value;
        return null;
    }
    public class CollegeResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AdminName { get; set; }
        public int YearOfStudy { get; set; }
        public int StudentCount { get; set; }
        public int ProfessorCount { get; set; }
    }
    public class CollegeDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int YearOfStudy { get; set; }
        public int AdminId { get; set; }

    }
}
