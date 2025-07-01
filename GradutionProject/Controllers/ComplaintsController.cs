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
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComplaintDto>>> GetAll(int pageNumber = 1, int pageSize = 10)
    {
        var CollegeId = GetUserClaims();


        if (CollegeId is null)
            return Unauthorized(new { message = "Invalid token claims." });

        try
        {
            var totalCount = await _db.Complaints.CountAsync();

            if (totalCount == 0)
                return NoContent();

            var result = await _db.Complaints
                .OrderBy(c => c.CreatedDate) 
                .Where(x=>x.Student.CollegeId == CollegeId.CollegeId)
                .Include(x=>x.Student)
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
    [HttpGet("superadmin")]
    public async Task<ActionResult<IEnumerable<ComplaintDto>>> GetAllBySuperAdmin(int pageNumber = 1, int pageSize = 10)
    {

        try
        {
            var totalCount = await _db.Complaints.CountAsync();

            if (totalCount == 0)
                return NoContent();

            var result = await _db.Complaints
                .OrderBy(c => c.CreatedDate)
                .Include(x => x.Student)
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
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Create(CreateComplaintDto complaintDto)
    {
        var studentId = GetUserClaims();

        if (studentId == null )
            return Unauthorized(new { message = "Invalid token claims." });
        var complaint = new Complaint
        {
            StudentId = (int)studentId.UserId,
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
