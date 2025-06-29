using AutoMapper;
using GradutionProject.Data;
using GradutionProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CoursesController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/Courses?search=math&page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = _context.Courses
            .Include(c => c.College)
            .Include(c => c.Professsor)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.Contains(search));

        var totalCount = await query.CountAsync();
        var courses = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = _mapper.Map<List<CoursDto>>(courses);

        return Ok(new
        {
            totalCount,
            page,
            pageSize,
            data = result
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var course = await _context.Courses
            .Include(c => c.College)
            .Include(c => c.Professsor)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound(new { message = "Course not found." });

        return Ok(_mapper.Map<CoursDto>(course));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateCoursDto dto)
    {
        var collegeId = GetClaimValue("CollegeId");
        if (collegeId == null)
            return Unauthorized(new { message = "Invalid token claims." });

        if (dto.ProfessorId.HasValue && !await _context.Professors.AnyAsync(p => p.Id == dto.ProfessorId))
            return NotFound(new { message = "Professor not found." });

        var course = _mapper.Map<Cours>(dto);
        course.CollegeId = collegeId;

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = course.Id }, _mapper.Map<CoursDto>(course));
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] CreateCoursDto dto)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            return NotFound(new { message = "Course not found." });

        // فقط عدل إذا تم إرسال ProfessorId وكان مختلفًا
        if (dto.ProfessorId.HasValue && dto.ProfessorId != course.ProfessorId)
        {
            course.ProfessorId = dto.ProfessorId;
        }

        // يتم تجاهل التعديل لو كانت القيمة null أو نفسها
        _mapper.Map(dto, course);
        await _context.SaveChangesAsync();

        return NoContent();
    }



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

public class CoursDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal DegreeOfLabs { get; set; }
    public decimal DegreeOfStudiom { get; set; }
    public int Year { get; set; }
    public string CollegeName { get; set; }
    public string ProfessorName { get; set; }
}

public class CreateCoursDto
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal DegreeOfLabs { get; set; }
    public decimal DegreeOfStudiom { get; set; }
    public int Year { get; set; }
    public int? ProfessorId { get; set; }
}
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Cours, CoursDto>()
            .ForMember(dest => dest.CollegeName, opt => opt.MapFrom(src => src.College.Name))
            .ForMember(dest => dest.ProfessorName, opt => opt.MapFrom(src => src.Professsor.Name));

        CreateMap<CreateCoursDto, Cours>();
    }
}
