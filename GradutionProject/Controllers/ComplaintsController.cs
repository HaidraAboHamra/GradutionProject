using GradutionProject.Data;
using GradutionProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GradutionProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ComplaintsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public ComplaintsController(ApplicationDbContext applicationDbContext)
    {
        _db = applicationDbContext;
    }
    [Authorize(Roles ="Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComplaintDto>>> GetAll(int pageNumber = 1, int pageSize = 10)
    {
        var CollegeId = GetClaimValue("CollegeId");
        var adminId = GetClaimValue("UserId");


        if (CollegeId is null || adminId is null )
            return Unauthorized(new { message = "Invalid token claims." });

        try
        {
            var totalCount = await _db.Complaints.CountAsync();

            if (totalCount == 0)
                return NoContent();

            var result = await _db.Complaints
                .OrderBy(c => c.CreatedDate) 
                .Where(x=>x.Student.CollegeId == CollegeId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var complaints = result.Select(s => new ComplaintDto
            {
                StudentName = s.Student.Name,
                Description = s.Description,
            });

            return Ok(new
            {
                totalCount,
                pageNumber,
                pageSize,
                data = complaints
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles ="Student")]
    [HttpPost]
    public async Task<ActionResult> Create(CreateComplaintDto complaintDto)
    {
        var studentId = GetClaimValue("UserId");

        if (studentId == null )
            return Unauthorized(new { message = "Invalid token claims." });
        var complaint = new Complaint
        {
            StudentId = (int)studentId,
            Description = complaintDto.Description,
        };
        try
        {
            _db.Complaints.Add(complaint);
            await _db.SaveChangesAsync();
            return Ok(complaintDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    private int? GetClaimValue(string claimType)
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == claimType);
        if (claim != null && int.TryParse(claim.Value, out int value))
            return value;
        return null;
    }
    public class CreateComplaintDto
    {
        public string Description { get; set; }
    }
    public class ComplaintDto
    {
        public string StudentName { get; set; }
        public string Description { get; set; }
    }
}
