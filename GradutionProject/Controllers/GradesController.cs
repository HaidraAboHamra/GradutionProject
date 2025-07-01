using GradutionProject.Data;
using GradutionProject.Entities;
using GradutionProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GradutionProject.Controllers
{
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

        #region Models

        public class GradeInputModel
        {
            [Required] public int StudentId { get; set; }
            [Range(0, 100)] public decimal DegreeOfLabs { get; set; }
            [Range(0, 100)] public decimal DegreeOfStudiom { get; set; }
        }

        public class GradeViewModel
        {
            public int GradeId { get; set; }
            public int CourseId { get; set; }
            public string CourseName { get; set; }
            public decimal? DegreeOfLabs { get; set; }
            public decimal? DegreeOfStudiom { get; set; }
        }

        public class GradeAppealInputModel
        {
            [Required]
            public int GradeId { get; set; }

            public bool AppealOnLabs { get; set; }
            public bool AppealOnStudiom { get; set; }

            public decimal? RequestedDegreeOfLabs { get; set; }
            public decimal? RequestedDegreeOfStudiom { get; set; }

            [Required]
            [MaxLength(1000)]
            public string Reason { get; set; }
        }

        public class GradeAppealViewModel
        {
            public int Id { get; set; }
            public int GradeId { get; set; }
            public int StudentId { get; set; }
            public int CourseId { get; set; }
            public string StudentName { get; set; }
            public string CourseName { get; set; }
            public bool AppealOnLabs { get; set; }
            public bool AppealOnStudiom { get; set; }
            public decimal? RequestedDegreeOfLabs { get; set; }
            public decimal? RequestedDegreeOfStudiom { get; set; }
            public string Reason { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? ResolvedAt { get; set; }
            public int? ResolvedByProfessorId { get; set; }
        }

        public class GradeAppealDecisionModel
        {
            [Required]
            public int AppealId { get; set; }

            [Required]
            public bool IsApproved { get; set; }

            public decimal? NewDegreeOfLabs { get; set; }
            public decimal? NewDegreeOfStudiom { get; set; }
        }

 

        #endregion

        #region Grade Endpoints

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddGrade([FromBody] GradeInputModel model)
        {
            var claims = GetUserClaims();
            if (claims.Role != "Professor")
                return Forbid();
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

        [HttpGet("transcript/{studentId:int}")]
        [Authorize(Roles = "Professor,Student")]
        public async Task<IActionResult> GetTranscript(
            int studentId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var uc = GetUserClaims();

            if (uc.Role == "Student" && uc.UserId != studentId)
                return Forbid();

            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.GradeOfStudents
                .Where(g => g.StudentId == studentId)
                .Include(g => g.Cours)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var grades = await query
                .OrderByDescending(g => g.Cours.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new GradeViewModel
                {
                    GradeId = g.Id,
                    CourseId = g.Cours.Id,
                    CourseName = g.Cours.Name,
                    DegreeOfLabs = g.DegreeOfLabs,
                    DegreeOfStudiom = g.DegreeOfStudiom
                })
                .ToListAsync();

            var result = new PagedResult<GradeViewModel>
            {
                Items = grades,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
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

        #endregion

        #region Grade Appeals Endpoints

        [HttpPost("appeal")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitGradeAppeal([FromBody] GradeAppealInputModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var uc = GetUserClaims();
            if (uc.UserId == null)
                return Unauthorized();

            var grade = await _context.GradeOfStudents
                .Include(g => g.Cours)
                .FirstOrDefaultAsync(g => g.Id == model.GradeId && g.StudentId == uc.UserId);

            if (grade == null)
                return NotFound("Grade not found or you don't have permission.");

            if (!model.AppealOnLabs && !model.AppealOnStudiom)
                return BadRequest("You must appeal on at least one type of grade (Labs or Studiom).");

            var appeal = new GradeAppeal
            {
                GradeOfStudentId = grade.Id,
                StudentId = uc.UserId.Value,
                CourseId = grade.Cours.Id,
                AppealOnLabs = model.AppealOnLabs,
                AppealOnStudiom = model.AppealOnStudiom,
                RequestedDegreeOfLabs = model.AppealOnLabs ? model.RequestedDegreeOfLabs : null,
                RequestedDegreeOfStudiom = model.AppealOnStudiom ? model.RequestedDegreeOfStudiom : null,
                Reason = model.Reason,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.GradeAppeals.Add(appeal);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Appeal submitted successfully.", AppealId = appeal.Id });
        }

        [HttpGet("appeals/pending")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> GetPendingAppeals(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var uc = GetUserClaims();
            if (uc.UserId == null)
                return Unauthorized();

            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.GradeAppeals
                .Include(a => a.GradeOfStudent)
                    .ThenInclude(g => g.Cours)
                .Include(a => a.GradeOfStudent)
                    .ThenInclude(g => g.Student)
                .Where(a => a.Status == "Pending" && a.GradeOfStudent.Cours.ProfessorId == uc.UserId)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var appeals = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new GradeAppealViewModel
                {
                    Id = a.Id,
                    GradeId = a.GradeOfStudentId,
                    StudentId = a.StudentId,
                    StudentName = a.GradeOfStudent.Student.Name,
                    CourseId = a.CourseId,
                    CourseName = a.GradeOfStudent.Cours.Name,
                    AppealOnLabs = a.AppealOnLabs ?? false,
                    AppealOnStudiom = a.AppealOnStudiom ?? false,
                    RequestedDegreeOfLabs = a.RequestedDegreeOfLabs,
                    RequestedDegreeOfStudiom = a.RequestedDegreeOfStudiom,
                    Reason = a.Reason,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt,
                    ResolvedAt = a.ResolvedAt,
                    ResolvedByProfessorId = a.ResolvedByProfessorId
                })
                .ToListAsync();

            var result = new PagedResult<GradeAppealViewModel>
            {
                Items = appeals,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }

        [HttpPost("appeals/decision")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> DecideOnAppeal([FromBody] GradeAppealDecisionModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var uc = GetUserClaims();
            if (uc.UserId == null)
                return Unauthorized();

            var appeal = await _context.GradeAppeals
                .Include(a => a.GradeOfStudent)
                .ThenInclude(g => g.Cours)
                .FirstOrDefaultAsync(a => a.Id == model.AppealId);

            if (appeal == null)
                return NotFound("Appeal not found.");

            if (appeal.Status != "Pending")
                return BadRequest("Appeal already decided.");

            if (appeal.GradeOfStudent.Cours.ProfessorId != uc.UserId)
                return Forbid();

            if (model.IsApproved)
            {
                if (model.NewDegreeOfLabs.HasValue)
                    appeal.GradeOfStudent.DegreeOfLabs = model.NewDegreeOfLabs.Value;
                if (model.NewDegreeOfStudiom.HasValue)
                    appeal.GradeOfStudent.DegreeOfStudiom = model.NewDegreeOfStudiom.Value;

                appeal.Status = "Approved";
                appeal.ResolvedAt = DateTime.Now;
                appeal.ResolvedByProfessorId = uc.UserId;

                _context.GradeOfStudents.Update(appeal.GradeOfStudent);
            }
            else
            {
                appeal.Status = "Rejected";
                appeal.ResolvedAt = DateTime.Now;
                appeal.ResolvedByProfessorId = uc.UserId;
            }

            _context.GradeAppeals.Update(appeal);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Appeal {(model.IsApproved ? "approved" : "rejected")}." });
        }

        #endregion
    }

    public class UserClaims
    {
        public int? UserId { get; set; }
        public string Role { get; set; }
        public int? CollegeId { get; set; }
    }
}
