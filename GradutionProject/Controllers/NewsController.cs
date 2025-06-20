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
            var query = _db.News.AsQueryable();

            if (date.HasValue)
            {
                var targetDate = date.Value.Date;
                query = query.Where(n => n.CreatedDate.Date == targetDate);
            }

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
                    AdminName = n.Admin.Name 
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
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> CreateNews([FromBody] CreateNewsDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Description))
            return BadRequest(new { message = "Description is required." });

        try
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim))
                return Unauthorized(new { message = "AdminId not found in token." });

            int adminId = int.Parse(adminIdClaim);

            var news = new News
            {
                Description = dto.Description,
                AdminId = adminId,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };

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

    public class CreateNewsDto
    {
        public string Description { get; set; }
    }

}
