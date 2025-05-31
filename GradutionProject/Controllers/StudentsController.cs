using GradutionProject.Data;
using GradutionProject.Entities;
using GradutionProject.Entities.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GradutionProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController(ApplicationDbContext context, IWebHostEnvironment environment) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        // GET: api/student
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAll()
        {
            var students = await _context.Students.ToListAsync();
            return Ok(students);
        }

        // GET: api/student/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetById(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return NotFound(new { message = "Student not found." });

            return Ok(student);
        }

        // POST: api/student/register
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromForm] StudentRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            byte[]? certificateBytes = await ConvertToBytes(dto.CertificateImg);
            byte[]? personalBytes = await ConvertToBytes(dto.PersonalPhoto);
            byte[]? invoiceBytes = await ConvertToBytes(dto.Invoice);

            var student = new Student
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                Gender = dto.Gender.HasValue ? (Gender?)dto.Gender : null,
                College = dto.College,
                PhoneNumber = dto.PhoneNumber,
                Birth = dto.Birth,
                CertificateDate = dto.CertificateDate,
                NationalId = dto.NationalId,
                CertificateImg = certificateBytes,
                PersonalPhoto = personalBytes,
                Invoice = invoiceBytes
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Student registered successfully", student.Id });
        }

        // PUT: api/student/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] StudentRegisterDto dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return NotFound(new { message = "Student not found." });

            student.Name = dto.Name;
            student.Email = dto.Email;
            student.PasswordHash = dto.PasswordHash;
            student.Gender = dto.Gender.HasValue ? (Gender?)dto.Gender : null;
            student.College = dto.College;
            student.PhoneNumber = dto.PhoneNumber;
            student.Birth = dto.Birth;
            student.CertificateDate = dto.CertificateDate;
            student.NationalId = dto.NationalId;

            if (dto.CertificateImg != null)
                student.CertificateImg = await ConvertToBytes(dto.CertificateImg);

            if (dto.PersonalPhoto != null)
                student.PersonalPhoto = await ConvertToBytes(dto.PersonalPhoto);

            if (dto.Invoice != null)
                student.Invoice = await ConvertToBytes(dto.Invoice);

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Student updated successfully", student });
        }

        // DELETE: api/student/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return NotFound(new { message = "Student not found." });

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Student deleted successfully" });
        }

        private async Task<byte[]?> ConvertToBytes(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }
    }

    // DTO
    public class StudentRegisterDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public int? Gender { get; set; } // 0: Male, 1: Female
        public string? College { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birth { get; set; }
        public DateTime? CertificateDate { get; set; }
        public string? NationalId { get; set; }

        public IFormFile? CertificateImg { get; set; }
        public IFormFile? PersonalPhoto { get; set; }
        public IFormFile? Invoice { get; set; }
    }
}
