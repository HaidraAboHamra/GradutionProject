using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using GradutionProject.Data;
using GradutionProject.Entities;
using GradutionProject.Abstractions;
using Error = GradutionProject.Abstractions.Error;

public class UserService
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher<Student> _passwordHasher;
    private readonly PasswordHasher<Admin> _passwordHasherAdmin;
    private readonly PasswordHasher<Professor> _passwordHasherProfessor;



    public UserService(ApplicationDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<Student>();
        _passwordHasherAdmin = new PasswordHasher<Admin>();
    }

    public async Task<Student> CreateUserAsync(Student student)
    {
        student.PasswordHash = _passwordHasher.HashPassword(student, student.PasswordHash);
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return student;
    }

    public async Task<Result<Student>> GetById(int id)
    {
        var user = await _context.Students.FirstOrDefaultAsync(x => x.Id == id);
        if (user is null)
        {
            return Result<Student>.Failure(new Error("Can't find a user with this id"));
        }
        return Result<Student>.Success(user);
    }

    public async Task<Student?> LoginAsync(string email, string password)
    {
        var student = await _context.Students.FirstOrDefaultAsync(u => u.Email == email);

        if (student == null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(student, student.PasswordHash, password);

        return result == PasswordVerificationResult.Success ? student : null;
    }
    public async Task<Admin?> LoginAdminAsync(string email, string password)
    {
        var admin = await _context.Admins.FirstOrDefaultAsync(u => u.Email == email);

        if (admin == null)
        {
            return null;
        }

        var result = _passwordHasherAdmin.VerifyHashedPassword(admin, admin.Password, password);

        return result == PasswordVerificationResult.Success ? admin : null;
    }
    public async Task<Professor?> LoginProfessorAsync(string email, string password)
    {
        var professor = await _context.Professors.FirstOrDefaultAsync(u => u.Email == email);

        if (professor == null)
        {
            return null;
        }

        var result = _passwordHasherProfessor.VerifyHashedPassword(professor, professor.Password, password);

        return result == PasswordVerificationResult.Success ? professor : null;
    }

    public async Task<Result> ChangePasswordAsync(int studentId, string currentPassword, string newPassword)
    {
        var student = await _context.Students.FirstOrDefaultAsync(u => u.Id == studentId);
        if (student == null)
        {
            return Result.Failure(new Error("User not found"));
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(student, student.PasswordHash, currentPassword);
        if (passwordVerificationResult != PasswordVerificationResult.Success)
        {
            return Result.Failure(new Error("Current password is incorrect"));
        }

        student.PasswordHash = _passwordHasher.HashPassword(student, newPassword);
        _context.Students.Update(student);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

}
