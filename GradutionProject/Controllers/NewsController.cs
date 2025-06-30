using GradutionProject.Data;
using GradutionProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GradutionProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public NewsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll(
        int pageNumber = 1,
        int pageSize = 10,
        DateTime? date = null)
    {
        try
        {
            var userClaims = GetUserClaims();

            if (!userClaims.CollegeId.HasValue)
                return Forbid("User does not have a valid CollegeId claim.");

            var collegeId = userClaims.CollegeId.Value;

            var query = _db.News
                .Include(n => n.Admin).ThenInclude(x=>x.College)
                .Include(n => n.Professor)
                .AsQueryable();

            if (date.HasValue)
            {
                var targetDate = date.Value.Date;
                query = query.Where(n => n.CreatedDate.Date == targetDate);
            }

            query = query.Where(n =>
                (n.Admin != null && n.Admin.College.Id == collegeId) ||
                (n.Professor != null && n.Professor.CollegeId == collegeId));

            var totalCount = await query.CountAsync();

            if (totalCount == 0)
                return NoContent();

            var newsList = await query
                .OrderByDescending(n => n.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new
                {
                    n.Description,
                    n.CreatedDate,
                    PublisherName = n.Admin != null ? n.Admin.Name :
                                    n.Professor != null ? n.Professor.Name : "Unknown"
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                pageNumber,
                pageSize,
                data = newsList
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [Authorize(Roles = "Admin,Professor")]
    [HttpPost]
    public async Task<ActionResult> CreateNews([FromBody] CreateNewsDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Description))
            return BadRequest(new { message = "Description is required." });

        try
        {
            var news = new News
            {
                Description = dto.Description,
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now
            };

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
                return Unauthorized(new { message = "Invalid token data." });

            if (userRole == "Admin")
            {
                news.AdminId = int.Parse(userId);
            }
            else if (userRole == "Professor")
            {
                news.ProfessorId = int.Parse(userId);
            }
            else
            {
                return Forbid();
            }

            _db.News.Add(news);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "News added successfully.",
                news.Description,
                news.CreatedDate
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int? GetClaimValue(string claimType)
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == claimType);
        if (claim != null && int.TryParse(claim.Value, out int value))
            return value;
        return null;
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
    public class CreateNewsDto
    {
        public string Description { get; set; }
    }

}
