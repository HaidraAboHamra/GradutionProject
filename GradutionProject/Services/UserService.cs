using GradutionProject.Data;
using GradutionProject.Entities;
using GradutionProject.Abstractions;
using GradutionProject.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GradutionProject.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IEmailService _emailService;

    public UserService(ApplicationDbContext context, IPasswordService passwordService, IEmailService emailService)
    {
        _context = context;
        _passwordService = passwordService;
        _emailService = emailService;
    }

    public async Task<Student> CreateUserAsync(Student student)
    {
        student.PasswordHash = _passwordService.HashPassword(student.PasswordHash);
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Result<Student>> GetById(int id)
    {
        var user = await _context.Students.FirstOrDefaultAsync(x => x.Id == id);
        if (user is null)
            return Result<Student>.Failure(new Error("Can't find a user with this id"));

        return Result<Student>.Success(user);
    }

    public async Task<Student?> LoginAsync(string email, string password)
    {
        var student = await _context.Students.FirstOrDefaultAsync(u => u.Email == email);
        if (student == null || !_passwordService.VerifyPassword(student.PasswordHash, password))
            return null;

        return student;
    }

    public async Task<Admin?> LoginAdminAsync(string email, string password)
    {
        var admin = await _context.Admins.FirstOrDefaultAsync(u => u.Email == email);
        if (admin == null || !_passwordService.VerifyPassword(admin.Password, password))
            return null;

        return admin;
    }

    public async Task<Professor?> LoginProfessorAsync(string email, string password)
    {
        var professor = await _context.Professors.FirstOrDefaultAsync(u => u.Email == email);
        if (professor == null || !_passwordService.VerifyPassword(professor.Password, password))
            return null;

        return professor;
    }

    public async Task<Result> ChangePasswordAsync(int studentId, string newPassword)
    {
        var student = await _context.Students.FirstOrDefaultAsync(u => u.Id == studentId);
        if (student == null)
            return Result.Failure(new Error("User not found"));

        student.PasswordHash = _passwordService.HashPassword(newPassword);
        _context.Students.Update(student);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<int?> GetAdminCollegeId(int id)
    {
        try
        {
            var college = await _context.Colleges.FirstOrDefaultAsync(x => x.AdminId == id);
            return college?.Id ?? 0;
        }
        catch
        {
            return 0;
        }
    }

    public async Task<int?> GetAdmCollegeId(int id)
    {
        return await GetAdminCollegeId(id);
    }
    public async Task<Result> ChangePasswordByUserAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _context.Students.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Result.Failure(new Error("User not found"));

        if (!_passwordService.VerifyPassword(user.PasswordHash, currentPassword))
            return Result.Failure(new Error("Current password is incorrect"));

        user.PasswordHash = _passwordService.HashPassword(newPassword);
        _context.Students.Update(user);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> ChangePasswordByAdminAsync(int adminId, int targetUserId, string newPassword, string targetRole)
    {

        if (targetRole.Equals("Student", StringComparison.OrdinalIgnoreCase))
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == targetUserId);
            if (student == null)
                return Result.Failure(new Error("Student not found"));

            student.PasswordHash = _passwordService.HashPassword(newPassword);
            _context.Students.Update(student);
            await _emailService.SendEmailAsync(student.Email,"Change Your Password",$"Your New Password Is: {newPassword}");

        }
        else if (targetRole.Equals("Professor", StringComparison.OrdinalIgnoreCase))
        {
            var professor = await _context.Professors.FirstOrDefaultAsync(p => p.Id == targetUserId);
            if (professor == null)
                return Result.Failure(new Error("Professor not found"));
            try
            {
                professor.Password = _passwordService.HashPassword(newPassword);
                _context.Professors.Update(professor);
            }
            catch
            {

            }

        }
        else
        {
            return Result.Failure(new Error("Invalid target role"));
        }

        await _context.SaveChangesAsync();
        return Result.Success();
    }
}
