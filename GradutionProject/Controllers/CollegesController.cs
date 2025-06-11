using GradutionProject.Data;
using GradutionProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace GradutionProject.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CollegesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CollegesController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _context.Colleges.ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var college = await _context.Colleges.Include(c => c.Students).Include(c => c.Professsors).FirstOrDefaultAsync(c => c.Id == id);
        return college == null ? NotFound() : Ok(college);
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
        if (id != updatedCollege.Id) return BadRequest();
        _context.Entry(updatedCollege).State = EntityState.Modified;
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
    public class CollegeDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int YearOfStudy { get; set; }
        public int AdminId { get; set; }

    }
}
