using GradutionProject.Data;
using GradutionProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GradutionProject.Controllers;
[Route("api/[controller]")]
[ApiController]
public class LectureController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LectureController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Lecture?query=someText&pageNumber=1&pageSize=10
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LectureDto>>> GetLectures(
        [FromQuery] string? query,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var claims = GetUserClaims();

        if (claims is null)
            return Unauthorized(new { message = "Invalid token claims." });

        var lecturesQuery = _context.Lectures
            .Include(x => x.College)
            .Include(x => x.Professsor).ThenInclude(x => x.Cours)
            .Where(l => l.CollegeId == claims.CollegeId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            query = query.Trim().ToLower();
            lecturesQuery = lecturesQuery.Where(l =>
                l.Name.ToLower().Contains(query) ||
                (l.Description != null && l.Description.ToLower().Contains(query))
            );
        }

        var totalCount = await lecturesQuery.CountAsync();

        var lectures = await lecturesQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = lectures.Select(l => new LectureDto
        {
            Id = l.Id,
            Name = l.Name,
            Description = l.Description,
            NumberOfLectures = l.NumberOfLectures,
            ProfessorId = l.ProfessorId,
            CollegeId = l.CollegeId,
            ProfessorName = l.Professsor.Name,
            CoursName = l.Professsor.Cours.Name,
            PdfBase64 = l.Pdf != null ? Convert.ToBase64String(l.Pdf) : null
        });

        return Ok(new
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Data = result
        });
    }

    // POST: api/Lecture
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<LectureDto>> CreateLecture([FromForm] CreateLectureDto dto)
    {
        var claims = GetUserClaims();


        if (claims is null)
            return Unauthorized(new { message = "Invalid token claims." });

        byte[]? pdf = await ConvertToBytes(dto.Pdf);

        var lecture = new Lecture
        {
            Name = dto.Name,
            Description = dto.Description,
            NumberOfLectures = dto.NumberOfLectures,
            Pdf = pdf,
            CollegeId = claims.CollegeId,
            ProfessorId = claims.UserId
        };

        _context.Lectures.Add(lecture);
        await _context.SaveChangesAsync();

        var result = new LectureDto
        {
            Id = lecture.Id,
            Name = lecture.Name,
            Description = lecture.Description,
            NumberOfLectures = lecture.NumberOfLectures,
            CollegeId = lecture.CollegeId,
            ProfessorId = lecture.ProfessorId,
            PdfBase64 = lecture.Pdf != null ? Convert.ToBase64String(lecture.Pdf) : null
        };

        return CreatedAtAction(nameof(GetLecture), new { id = lecture.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LectureDto>> GetLecture(int id)
    {
        var lecture = await _context.Lectures.FindAsync(id);

        if (lecture == null)
            return NotFound();

        var dto = new LectureDto
        {
            Id = lecture.Id,
            Name = lecture.Name,
            Description = lecture.Description,
            NumberOfLectures = lecture.NumberOfLectures,
            CollegeId = lecture.CollegeId,
            ProfessorId = lecture.ProfessorId,
            ProfessorName = lecture.Professsor.Name,
            PdfBase64 = lecture.Pdf != null ? Convert.ToBase64String(lecture.Pdf) : null
        };

        return Ok(dto);
    }

    // PUT: api/Lecture/5
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLecture(int id, [FromForm] CreateLectureDto dto)
    {
        var lecture = await _context.Lectures.FindAsync(id);
        if (lecture == null)
            return NotFound();

        var professorId = GetClaimValue("UserId");
        var collegeId = GetClaimValue("CollegeId");

        if (professorId == null || collegeId == null)
            return Unauthorized(new { message = "Invalid token claims." });

        byte[]? pdf = await ConvertToBytes(dto.Pdf);

        lecture.Name = dto.Name;
        lecture.Description = dto.Description;
        lecture.NumberOfLectures = dto.NumberOfLectures;
        lecture.Pdf = pdf;
        lecture.CollegeId = collegeId;
        lecture.ProfessorId = professorId;

        _context.Entry(lecture).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<byte[]?> ConvertToBytes(IFormFile? file)
    {
        if (file == null || file.Length == 0) return null;

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        return ms.ToArray();
    }
    // DELETE: api/Lecture/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLecture(int id)
    {
        var lecture = await _context.Lectures.FindAsync(id);
        if (lecture == null)
            return NotFound();

        _context.Lectures.Remove(lecture);
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
    public class UserClaims
    {
        public int? UserId { get; set; }
        public string Role { get; set; }
        public int? CollegeId { get; set; }
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

    // LectureDto.cs
    public class LectureDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfLectures { get; set; }
        public int? CollegeId { get; set; }
        public int? ProfessorId { get; set; }
        public string? ProfessorName { get; set; }
        public string? CoursName { get; set; }



        // PDF كـ Base64
        public string? PdfBase64 { get; set; }
    }


    // CreateLectureDto.cs
    public class CreateLectureDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfLectures { get; set; }
        public IFormFile Pdf { get; set; }
    }

}
