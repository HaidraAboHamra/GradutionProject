using GradutionProject.Data;
using GradutionProject.Entities;
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

    // GET: api/Lecture
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LectureDto>>> GetLectures()
    {
        return await _context.Lectures
            .Select(l => new LectureDto
            {
                Id = l.Id,
                Name = l.Name,
                Description = l.Description,
                NumberOfLectures = l.NumberOfLectures,
                CollegeId = l.CollegeId,
                ProfessorId = l.ProfessorId
            }).ToListAsync();
    }

    // GET: api/Lecture/5
    [HttpGet("{id}")]
    public async Task<ActionResult<LectureDto>> GetLecture(int id)
    {
        var lecture = await _context.Lectures.FindAsync(id);

        if (lecture == null)
            return NotFound();

        return new LectureDto
        {
            Id = lecture.Id,
            Name = lecture.Name,
            Description = lecture.Description,
            NumberOfLectures = lecture.NumberOfLectures,
            CollegeId = lecture.CollegeId,
            ProfessorId = lecture.ProfessorId
        };
    }

    // POST: api/Lecture
    [HttpPost]
    public async Task<ActionResult<LectureDto>> CreateLecture([FromBody] CreateLectureDto dto)
    {
        var lecture = new Lecture
        {
            Name = dto.Name,
            Description = dto.Description,
            NumberOfLectures = dto.NumberOfLectures,
            Pdf = dto.Pdf,
            CollegeId = dto.CollegeId,
            ProfessorId = dto.ProfessorId
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
            ProfessorId = lecture.ProfessorId
        };

        return CreatedAtAction(nameof(GetLecture), new { id = lecture.Id }, result);
    }

    // PUT: api/Lecture/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLecture(int id, [FromBody] CreateLectureDto dto)
    {
        var lecture = await _context.Lectures.FindAsync(id);
        if (lecture == null)
            return NotFound();

        lecture.Name = dto.Name;
        lecture.Description = dto.Description;
        lecture.NumberOfLectures = dto.NumberOfLectures;
        lecture.Pdf = dto.Pdf;
        lecture.CollegeId = dto.CollegeId;
        lecture.ProfessorId = dto.ProfessorId;

        _context.Entry(lecture).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
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
    // LectureDto.cs
    public class LectureDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfLectures { get; set; }
        public int? CollegeId { get; set; }
        public int? ProfessorId { get; set; }
    }

    // CreateLectureDto.cs
    public class CreateLectureDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfLectures { get; set; }
        public byte[] Pdf { get; set; }
        public int? CollegeId { get; set; }
        public int? ProfessorId { get; set; }
    }

}
