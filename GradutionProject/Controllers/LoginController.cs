using GradutionProject.Abstractions;
using GradutionProject.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
[Route("api/[controller]")]
[ApiController]
public class LoginController(UserService _userService, IConfiguration _configuration) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var email = request.Email.ToLower();
        var result = email.Contains("admin")
            ? await AdminLoginAsync(request.Email, request.Password)
            : email.Contains("prof")
                ? await ProfLoginAsync(request.Email, request.Password)
                : await StudentLoginAsync(request.Email, request.Password);

        return result.ToActionResult();
    }

    public Task<Result<object>> AdminLoginAsync(string email, string password) =>
        AdminLogin(email, password, "Admin");

    public Task<Result<object>> ProfLoginAsync(string email, string password) =>
        PerformLogin(email, password, "Professor");

    public Task<Result<object>> StudentLoginAsync(string email, string password) =>
        PerformLogin(email, password, "User");

    private async Task<Result<object>> PerformLogin(string email, string password, string role)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Result.Failure<object>(new Error("Email and password are required"));

        var student = await _userService.LoginAsync(email, password);
        if (student == null)
            return Result.Failure<object>(new Error("Invalid email or password"));

        var token = CreateToken(student, role);
        return Result.Success<object>(new
        {
            Message = "Login successful",
            UserId = student.Id,
            Token = token,
            Role = role
        });
    }
    private async Task<Result<object>> AdminLogin(string email, string password, string role)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Result.Failure<object>(new Error("Email and password are required"));

        var student = await _userService.LoginAdminAsync(email, password);
        if (student == null)
            return Result.Failure<object>(new Error("Invalid email or password"));

        var token = CreateTokenAdmin(student, role);
        return Result.Success<object>(new
        {
            Message = "Login successful",
            UserId = student.Id,
            Token = token,
            Role = role
        });
    }
    private string CreateToken(Student user, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(2), signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private string CreateTokenAdmin(Admin user, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.MobilePhone, user.Phone ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(2), signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result) =>
        result.IsSuccess ? new OkResult() : new BadRequestObjectResult(new { Error = (string)result.Error });

    public static IActionResult ToActionResult<T>(this Result<T> result) =>
        result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { Error = (string)result.Error });
}
