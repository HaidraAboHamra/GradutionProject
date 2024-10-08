using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GradutionProject.Abstractions;
using GradutionProject.Data;
using GradutionProject.Entity;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserService(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    /// <summary>
    /// إنشاء مستخدم جديد.
    /// </summary>
    public async Task<User> CreateUserAsync(User user)
    {
        // تجزئة كلمة المرور
        user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);

        // إضافة المستخدم إلى قاعدة البيانات
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// استرجاع مستخدم بناءً على المعرف.
    /// </summary>
    public async Task<Result<User>> GetById(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user is null)
        {
            return Result<User>.Failure(new Error("Can't find a user with this id"));
        }
        return Result<User>.Success(user);
    }

    /// <summary>
    /// تسجيل دخول المستخدم.
    /// </summary>
    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        // التحقق من وجود المستخدم
        if (user == null)
        {
            return null;
        }

        // التحقق من صحة كلمة المرور
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        return result == PasswordVerificationResult.Success ? user : null;
    }
}
