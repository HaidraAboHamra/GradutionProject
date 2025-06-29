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
        private readonly IEmailService _emailService;


        public StudentService(ApplicationDbContext context, IPasswordService passwordService, IEmailService emailService)
        {
            _context = context;
            _passwordService = passwordService;
            _emailService = emailService;
        }

        public async Task<IEnumerable<StudentDto>> GetAllAsync(int collegeId, int page, int pageSize)
        {
            var students = await _context.NewStudents
                .Include(s => s.College)
                .Where(x => x.CollegeId == collegeId)
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
        public async Task<IEnumerable<StudentDto>> GetAllStudentAsync(int collegeId, int page, int pageSize)
        {
            var students = await _context.Students
                .Include(s => s.College)
                .Where(x => x.CollegeId == collegeId)
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
        public async Task<List<StudentDto>> SearchByNameAsync(int collegeId, string name)
        {
            var query = _context.Students
                .Include(s => s.College)
                .Where(x => x.CollegeId == collegeId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var loweredName = name.ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(loweredName));
            }

            var students = await query.Select(student => new StudentDto
            {
                Name = student.Name,
                Email = student.Email,
                Gender = (int?)student.Gender,
                CollegeId = student.College.Name,
                PhoneNumber = student.PhoneNumber,
                Birth = student.Birth,
                CertificateDate = student.CertificateDate,
                NationalId = student.NationalId,
                CertificateImgBase64 = student.CertificateImg != null ? Convert.ToBase64String(student.CertificateImg) : null,
                PersonalPhotoBase64 = student.PersonalPhoto != null ? Convert.ToBase64String(student.PersonalPhoto) : null,
                InvoiceBase64 = student.Invoice != null ? Convert.ToBase64String(student.Invoice) : null
            }).ToListAsync();

            return students;
        }

        public async Task<int> RegisterAsync(StudentRegisterDto dto)
        {
            var student = new NewStudent
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = _passwordService.HashPassword(dto.NationalId!),
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
                CollegeId = newStudent.CollegeId,
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
            try
            {
                string subject = "Your Authontication Details";
                string body = $"Dear {student.Name},\n Your Email is {student.Email} And Your Password Is {student.NationalId} \n Please Change Your Password in First Login";
                await _emailService.SendEmailAsync(student.Email, subject, body);
            }
            catch(Exception ex)
            {
                return student.Id;
            }
            return student.Id;
        }

        public async Task<bool> UpdateAsync(int collegeId, int id, StudentRegisterDto dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student is null)
                return false;

            if (collegeId != student.CollegeId)
                return false;

            if (!string.IsNullOrWhiteSpace(dto.Name) && student.Name != dto.Name)
                student.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Email) && student.Email != dto.Email)
                student.Email = dto.Email;

            if (dto.Gender != null && student.Gender != (Gender?)dto.Gender)
                student.Gender = (Gender?)dto.Gender;

            if (dto.College.HasValue && student.CollegeId != dto.College.Value)
                student.CollegeId = dto.College.Value;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && student.PhoneNumber != dto.PhoneNumber)
                student.PhoneNumber = dto.PhoneNumber;

            if (dto.Birth != null && student.Birth != dto.Birth)
                student.Birth = dto.Birth;

            if (dto.CertificateDate != null && student.CertificateDate != dto.CertificateDate)
                student.CertificateDate = dto.CertificateDate;

            if (!string.IsNullOrWhiteSpace(dto.NationalId) && student.NationalId != dto.NationalId)
                student.NationalId = dto.NationalId;

            if (dto.CertificateImg != null)
                student.CertificateImg = await ToBytes(dto.CertificateImg);

            if (dto.PersonalPhoto != null)
                student.PersonalPhoto = await ToBytes(dto.PersonalPhoto);

            if (dto.Invoice != null)
                student.Invoice = await ToBytes(dto.Invoice);

            if (!string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                var hashed = _passwordService.HashPassword(dto.PasswordHash);
                if (student.PasswordHash != hashed)
                    student.PasswordHash = hashed;
            }

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
        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.NewStudents.FindAsync(id);
            if (student == null)
                return false;

            _context.NewStudents.Remove(student);
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
