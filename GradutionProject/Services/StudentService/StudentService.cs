using GradutionProject.Controllers;
using GradutionProject.Data;
using GradutionProject.Entities;
using GradutionProject.Entities.Enums;
using GradutionProject.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GradutionProject.Services.StudentService
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;

        public StudentService(ApplicationDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<IEnumerable<StudentDto>> GetAllAsync(int page, int pageSize)
        {
            var students = await _context.NewStudents
                .Include(s => s.College)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return students.Select(s => new StudentDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                Gender = (int?)s.Gender,
                CollegeId = s.College?.Name,
                PhoneNumber = s.PhoneNumber,
                Birth = s.Birth,
                CertificateDate = s.CertificateDate,
                NationalId = s.NationalId,
                CertificateImgBase64 = s.CertificateImg != null ? Convert.ToBase64String(s.CertificateImg) : null,
                PersonalPhotoBase64 = s.PersonalPhoto != null ? Convert.ToBase64String(s.PersonalPhoto) : null,
                InvoiceBase64 = s.Invoice != null ? Convert.ToBase64String(s.Invoice) : null
            });
        }
        public async Task<IEnumerable<StudentDto>> GetAllStudentAsync(int page, int pageSize)
        {
            var students = await _context.Students
                .Include(s => s.College)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return students.Select(s => new StudentDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                Gender = (int?)s.Gender,
                CollegeId = s.College?.Name,
                PhoneNumber = s.PhoneNumber,
                Birth = s.Birth,
                CertificateDate = s.CertificateDate,
                NationalId = s.NationalId,
                CertificateImgBase64 = s.CertificateImg != null ? Convert.ToBase64String(s.CertificateImg) : null,
                PersonalPhotoBase64 = s.PersonalPhoto != null ? Convert.ToBase64String(s.PersonalPhoto) : null,
                InvoiceBase64 = s.Invoice != null ? Convert.ToBase64String(s.Invoice) : null
            });
        }

        public async Task<StudentDto?> GetByIdAsync(int id)
        {
            var s = await _context.Students
                .Include(s => s.College)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (s == null) return null;

            return new StudentDto
            {
                Name = s.Name,
                Email = s.Email,
                Gender = (int?)s.Gender,
                CollegeId = s.College?.Name,
                PhoneNumber = s.PhoneNumber,
                Birth = s.Birth,
                CertificateDate = s.CertificateDate,
                NationalId = s.NationalId,
                CertificateImgBase64 = s.CertificateImg != null ? Convert.ToBase64String(s.CertificateImg) : null,
                PersonalPhotoBase64 = s.PersonalPhoto != null ? Convert.ToBase64String(s.PersonalPhoto) : null,
                InvoiceBase64 = s.Invoice != null ? Convert.ToBase64String(s.Invoice) : null
            };
        }

        public async Task<int> RegisterAsync(StudentRegisterDto dto)
        {
            var student = new NewStudent
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = _passwordService.HashPassword(dto.PasswordHash!),
                Gender = (Gender?)dto.Gender,
                CollegeId = dto.College,
                PhoneNumber = dto.PhoneNumber,
                Birth = dto.Birth,
                CertificateDate = dto.CertificateDate,
                NationalId = dto.NationalId,
                CertificateImg = await ToBytes(dto.CertificateImg),
                PersonalPhoto = await ToBytes(dto.PersonalPhoto),
                Invoice = await ToBytes(dto.Invoice)
            };

            _context.NewStudents.Add(student);
            await _context.SaveChangesAsync();
            return student.Id;
        }

        public async Task<int> PromoteAsync(int newStudentId)
        {
            var newStudent = await _context.NewStudents.FindAsync(newStudentId);
            if (newStudent == null)
                throw new Exception("Student not found.");

            var student = new Student
            {
                Name = newStudent.Name,
                Email = newStudent.Email,
                PasswordHash = newStudent.PasswordHash,
                Gender = newStudent.Gender,
                YearOfStudy = newStudent.YearOfStudy,
                PhoneNumber = newStudent.PhoneNumber,
                Birth = newStudent.Birth,
                CertificateDate = newStudent.CertificateDate,
                NationalId = newStudent.NationalId,
                CertificateImg = newStudent.CertificateImg,
                PersonalPhoto = newStudent.PersonalPhoto,
                Invoice = newStudent.Invoice,
            };

            _context.Students.Add(student);
            _context.NewStudents.Remove(newStudent);
            await _context.SaveChangesAsync();

            return student.Id;
        }

        public async Task<bool> UpdateAsync(int id, StudentRegisterDto dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return false;

            student.Name = dto.Name;
            student.Email = dto.Email;
            student.PasswordHash = _passwordService.HashPassword(dto.PasswordHash!);
            student.Gender = (Gender?)dto.Gender;
            student.CollegeId = dto.College;
            student.PhoneNumber = dto.PhoneNumber;
            student.Birth = dto.Birth;
            student.CertificateDate = dto.CertificateDate;
            student.NationalId = dto.NationalId;

            if (dto.CertificateImg != null)
                student.CertificateImg = await ToBytes(dto.CertificateImg);

            if (dto.PersonalPhoto != null)
                student.PersonalPhoto = await ToBytes(dto.PersonalPhoto);

            if (dto.Invoice != null)
                student.Invoice = await ToBytes(dto.Invoice);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<byte[]?> ToBytes(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }
    }

}
