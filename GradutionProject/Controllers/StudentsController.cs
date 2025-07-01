using GradutionProject.Data;
using GradutionProject.Entities;
using GradutionProject.Entities.Enums;
using GradutionProject.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    private readonly ApplicationDbContext _context;
    public StudentController(IStudentService studentService, ApplicationDbContext context)
    {
        _studentService = studentService;
        _context = context;
    }
    private UserClaims GetUserClaims()
    {
        var claims = User.Claims;

        int? userId = null;
        int? collegeId = null;
        string role = null;

        var idClaim = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        if (idClaim != null && int.TryParse(idClaim.Value, out int id))
            userId = id;

        var roleClaim = claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        role = roleClaim?.Value;

        var collegeClaim = claims.FirstOrDefault(c => c.Type == "CollegeId");
        if (collegeClaim != null && int.TryParse(collegeClaim.Value, out int cId))
            collegeId = cId;

        return new UserClaims
        {
            UserId = userId,
            Role = role,
            CollegeId = collegeId
        };
    }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var collegeId = GetClaimValue("CollegeId");
        if (collegeId == null)
            return Unauthorized(new { message = "Invalid token claims." });

        var result = await _studentService.GetAllAsync((int)collegeId, page, pageSize);
        var totalCount = await _context.NewStudents.CountAsync(x => x.CollegeId == (int)collegeId);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return Ok(new
        {
            data = result,
            totalPages
        });
    }

    [HttpGet("accepted")]
    public async Task<IActionResult> GetAllStudent([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var collegeId = GetClaimValue("CollegeId");
        if (collegeId == null)
            return Unauthorized(new { message = "Invalid token claims." });

        var result = await _studentService.GetAllStudentAsync((int)collegeId, page, pageSize);
        var totalCount = await _context.Students.CountAsync(x => x.CollegeId == (int)collegeId);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return Ok(new
        {
            data = result,
            totalPages
        });
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var collegeId = GetClaimValue("CollegeId");

        if (collegeId == null)
            return Unauthorized(new { message = "Invalid token claims." });

        var result = await _studentService.GetByIdAsync(id);
        return result is null ? NotFound(new { message = "Student not found." }) : Ok(result);
    }
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var claims = GetUserClaims();

        if (claims is null || claims.UserId is null)
            return Unauthorized(new { message = "Invalid token claims." });

        var student = await _context.Students
            .Include(s => s.College)
            .FirstOrDefaultAsync(s => s.Id == claims.UserId.Value);

        if (student is null)
            return NotFound(new { message = "Student not found." });

        var result = new StudentDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            PhoneNumber = student.PhoneNumber,
            Birth = student.Birth,
            CertificateDate = student.CertificateDate,
            NationalId = student.NationalId,
            Gender = (int?)student.Gender,
            CollegeName = student.College?.Name
        };

        return Ok(result);
    }

    [HttpGet("search/{name}")]
    public async Task<IActionResult> GetByName(string name, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var collegeId = GetClaimValue("CollegeId");

        if (collegeId == null)
            return Unauthorized(new { message = "Invalid token claims." });

        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        var (students, totalCount) = await _studentService.SearchByNameAsync((int)collegeId, name, pageNumber, pageSize);

        if (students == null || students.Count == 0)
            return NotFound(new { message = "No students found with this name." });

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return Ok(new
        {
            data = students,
            currentPage = pageNumber,
            totalPages = totalPages
        });
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
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] StudentRegisterDto dto)
    {
        var collegeId = GetClaimValue("CollegeId");

        if (collegeId == null)
            return Unauthorized(new { message = "Invalid token claims." });

        if (!ModelState.IsValid) return BadRequest(ModelState);
        var success = await _studentService.UpdateAsync((int)collegeId, id, dto);
        return success ? Ok(new { message = "Updated" }) : NotFound(new { message = "Student not found." });
    }
    [HttpDelete("new/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _studentService.DeleteStudentAsync(id);
        return success ? Ok(new { message = "Deleted" }) : NotFound(new { message = "Student not found." });
    }

    [HttpDelete("registered/{id}")]
    public async Task<IActionResult> DeleteNewStudent(int id)
    {
        var success = await _studentService.DeleteAsync(id);
        return success ? Ok(new { message = "Deleted" }) : NotFound(new { message = "Student not found." });
    }
    private int? GetClaimValue(string claimType)
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == claimType);
        if (claim != null && int.TryParse(claim.Value, out int value))
            return value;
        return null;
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
    public int Id { get; set; }
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
    public string? CollegeName { get; internal set; }
}
