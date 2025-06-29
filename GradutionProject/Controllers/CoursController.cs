using GradutionProject.Data;
using GradutionProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradutionProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cours>>> GetAll()
        {
            var courses = await _context.Courses
                .Include(c => c.College)
                .Include(c => c.Professsor)
                .ToListAsync();

            return Ok(courses);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cours>> GetById(int id)
        {
            var course = await _context.Courses
                .Include(c => c.College)
                .Include(c => c.Professsor)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound(new { message = "Course not found." });

            return Ok(course);
        }

        // POST: api/Courses
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(Cours cours)
        {
            var collegeId = GetClaimValue("CollegeId");

            if (collegeId == null)
                return Unauthorized(new { message = "Invalid token claims." });
            
            if (cours == null)
                return BadRequest(new { message = "Invalid course data." });

            if (cours.ProfessorId.HasValue && !await _context.Professors.AnyAsync(p => p.Id == cours.ProfessorId))
                return NotFound(new { message = "Professor not found." });
            cours.CollegeId = collegeId;
            _context.Courses.Add(cours);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = cours.Id }, cours);
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Cours updatedCourse)
        {
            if (id != updatedCourse.Id)
                return BadRequest(new { message = "Mismatched course ID." });

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound(new { message = "Course not found." });

            course.Name = updatedCourse.Name;
            course.Description = updatedCourse.Description;
            course.DegreeOfLabs = updatedCourse.DegreeOfLabs;
            course.DegreeOfStudiom = updatedCourse.DegreeOfStudiom;
            course.CollegeId = updatedCourse.CollegeId;
            course.ProfessorId = updatedCourse.ProfessorId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound(new { message = "Course not found." });

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course deleted successfully." });
        }
        private int? GetClaimValue(string claimType)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == claimType);
            if (claim != null && int.TryParse(claim.Value, out int value))
                return value;
            return null;
        }
    }
}
