using GradutionProject.Data;
using GradutionProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComplaintDto>>> GetAll(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var totalCount = await _db.Complaints.CountAsync();

            if (totalCount == 0)
                return NoContent();

            var result = await _db.Complaints
                .OrderBy(c => c.CreatedDate) 
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var complaints = result.Select(s => new ComplaintDto
            {
                StudentId = s.StudentId,
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
    public async Task<ActionResult> Create(ComplaintDto complaintDto)
    {
        var complaint = new Complaint
        {
            StudentId = (int)complaintDto.StudentId,
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
    public class ComplaintDto
    {
        public int? StudentId { get; set; }
        public string Description { get; set; }
    }
}
