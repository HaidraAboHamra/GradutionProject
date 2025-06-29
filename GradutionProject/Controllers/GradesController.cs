using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GradutionProject.Controllers
{
    using global::GradutionProject.Data;
    using global::GradutionProject.Entities;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using static GradutionProject.Controllers.LectureController;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GradesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GradesController(ApplicationDbContext context)
            => _context = context;

        private UserClaims GetUserClaims()
        {
            var claims = User.Claims;
            int? userId = null;
            int? collegeId = null;
            string role = null;

            if (int.TryParse(claims.FirstOrDefault(c => c.Type ==
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value, out var id))
                userId = id;

            role = claims.FirstOrDefault(c => c.Type ==
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            if (int.TryParse(claims.FirstOrDefault(c => c.Type == "CollegeId")?.Value, out var cId))
                collegeId = cId;

            return new() { UserId = userId, Role = role, CollegeId = collegeId };
        }

        public class GradeInputModel
        {
            [Required] public int StudentId { get; set; }
            [Range(0, 100)] public decimal DegreeOfLabs { get; set; }
            [Range(0, 100)] public decimal DegreeOfStudiom { get; set; }
        }

        [HttpPost("add")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> AddGrade([FromBody] GradeInputModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var uc = GetUserClaims();
            if (uc.UserId == null) return Unauthorized();

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.ProfessorId == uc.UserId);

            if (course == null) return BadRequest("No course assigned to this professor.");

            if (!await _context.Students.AnyAsync(s => s.Id == model.StudentId))
                return NotFound("Student not found.");

            var exists = await _context.GradeOfStudents
                .AnyAsync(g => g.CoursesId == course.Id && g.StudentId == model.StudentId);

            if (exists) return Conflict("Grade already exists for this student in this course.");

            var grade = new GradeOfStudent
            {
                StudentId = model.StudentId,
                DegreeOfLabs = model.DegreeOfLabs,
                DegreeOfStudiom = model.DegreeOfStudiom,
                CoursesId = course.Id
            };

            _context.GradeOfStudents.Add(grade);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Grade added", GradeId = grade.Id });
        }

        public class GradeViewModel
        {
            public int GradeId { get; set; }
            public int CourseId { get; set; }
            public string CourseName { get; set; }
            public decimal? DegreeOfLabs { get; set; }
            public decimal? DegreeOfStudiom { get; set; }
        }

        [HttpGet("transcript/{studentId:int}")]
        [Authorize(Roles = "Professor,Student")]
        public async Task<IActionResult> GetTranscript(int studentId)
        {
            var uc = GetUserClaims();

            if (uc.Role == "Student" && uc.UserId != studentId)
                return Forbid();

            var grades = await _context.GradeOfStudents
                .Where(g => g.StudentId == studentId)
                .Include(g => g.Cours)
                .Select(g => new GradeViewModel
                {
                    GradeId = g.Id,
                    CourseId = g.Cours.Id,
                    CourseName = g.Cours.Name,
                    DegreeOfLabs = g.DegreeOfLabs,
                    DegreeOfStudiom = g.DegreeOfStudiom
                })
                .ToListAsync();

            return Ok(grades);
        }

        [HttpGet("{gradeId:int}")]
        [Authorize(Roles = "Professor,Student")]
        public async Task<IActionResult> GetGradeDetail(int gradeId)
        {
            var uc = GetUserClaims();

            var grade = await _context.GradeOfStudents
                .Include(g => g.Cours)
                .Include(g => g.Student)
                .FirstOrDefaultAsync(g => g.Id == gradeId);

            if (grade == null) return NotFound();

            if (uc.Role == "Professor" && grade.Cours.ProfessorId != uc.UserId)
                return Forbid();
            if (uc.Role == "Student" && grade.StudentId != uc.UserId)
                return Forbid();

            return Ok(new
            {
                grade.Id,
                Course = new { grade.Cours.Id, grade.Cours.Name },
                Student = new { grade.Student.Id, grade.Student.Name },
                grade.DegreeOfLabs,
                grade.DegreeOfStudiom
            });
        }
    }

}
