using GradutionProject.Data;
using GradutionProject.Entities;
using GradutionProject.Entities.Enums;
using GradutionProject.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GradutionProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;
    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _studentService.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _studentService.GetByIdAsync(id);
        return result is null ? NotFound(new { message = "Student not found." }) : Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] StudentRegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var id = await _studentService.RegisterAsync(dto);
        return Ok(new { message = "Student registered", id });
    }

    [HttpPost("promote/{id}")]
    public async Task<IActionResult> Promote(int id)
    {
        try
        {
            var newId = await _studentService.PromoteAsync(id);
            return Ok(new { message = "Promoted", id = newId });
        }
        catch
        {
            return NotFound(new { message = "Student not found." });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] StudentRegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var success = await _studentService.UpdateAsync(id, dto);
        return success ? Ok(new { message = "Updated" }) : NotFound(new { message = "Student not found." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _studentService.DeleteAsync(id);
        return success ? Ok(new { message = "Deleted" }) : NotFound(new { message = "Student not found." });
    }
}

// DTO
public class StudentRegisterDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public int? Gender { get; set; } // 0: Male, 1: Female
    public int? College { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? Birth { get; set; }
    public DateTime? CertificateDate { get; set; }
    public string? NationalId { get; set; }

    public IFormFile? CertificateImg { get; set; }
    public IFormFile? PersonalPhoto { get; set; }
    public IFormFile? Invoice { get; set; }
}
public class StudentDto
{

    public string? Name { get; set; }
    public string? Email { get; set; }
    public int? Gender { get; set; }
    public string? CollegeId { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? Birth { get; set; }
    public DateTime? CertificateDate { get; set; }
    public string? NationalId { get; set; }

    // صور Base64
    public string? CertificateImgBase64 { get; set; }
    public string? PersonalPhotoBase64 { get; set; }
    public string? InvoiceBase64 { get; set; }
}
