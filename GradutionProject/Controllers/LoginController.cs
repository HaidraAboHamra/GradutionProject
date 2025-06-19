using GradutionProject.Abstractions;
using GradutionProject.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;

    public LoginController(UserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var email = request.Email.ToLowerInvariant();

        Result<object> result;

        if (email.Contains("admin"))
        {
            result = await AdminLoginAsync(email, request.Password);
        }
        else if (email.Contains("prof"))
        {
            result = await ProfessorLoginAsync(email, request.Password);
        }
        else
        {
            result = await StudentLoginAsync(email, request.Password);
        }

        return result.ToActionResult();
    }

    private async Task<Result<object>> AdminLoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Result.Failure<object>(new Error("Email and password are required"));

        var admin = await _userService.LoginAdminAsync(email, password);
        if (admin == null)
            return Result.Failure<object>(new Error("Invalid email or password"));

        int? collegeId = admin.College?.Id;

        var token = GenerateJwtToken(admin.Id.ToString(), admin.Email, admin.Phone, "Admin", collegeId);

        return Result.Success<object>(new
        {
            Message = "Login successful",
            UserId = admin.Id,
            Role = "Admin",
            CollegeId = collegeId,
            Token = token
        });
    }

    private async Task<Result<object>> ProfessorLoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Result.Failure<object>(new Error("Email and password are required"));

        var professor = await _userService.LoginProfessorAsync(email, password);
        if (professor == null)
            return Result.Failure<object>(new Error("Invalid email or password"));

        int? collegeId = professor.CollegeId;

        
        var course = professor.Lectures?.FirstOrDefault(); 
        int? courseId = course?.Id;

        var token = GenerateJwtToken(professor.Id.ToString(), professor.Email, professor.Phone, "Professor", collegeId, courseId);

        return Result.Success<object>(new
        {
            Message = "Login successful",
            UserId = professor.Id,
            Role = "Professor",
            CollegeId = collegeId,
            CourseId = courseId,
            Token = token
        });
    }


    private async Task<Result<object>> StudentLoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Result.Failure<object>(new Error("Email and password are required"));

        var student = await _userService.LoginAsync(email, password);
        if (student == null)
            return Result.Failure<object>(new Error("Invalid email or password"));

        int? collegeId = student.CollegeId;

        var token = GenerateJwtToken(student.Id.ToString(), student.Email, student.PhoneNumber, "Student", collegeId);

        return Result.Success<object>(new
        {
            Message = "Login successful",
            UserId = student.Id,
            Role = "Student",
            CollegeId = collegeId,
            Token = token
        });
    }

    private string GenerateJwtToken(string userId, string email, string phone, string role, int? collegeId, int? courseId = null)
    {
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Email, email ?? string.Empty),
        new Claim(ClaimTypes.MobilePhone, phone ?? string.Empty),
        new Claim(ClaimTypes.Role, role),
    };

        if (collegeId.HasValue)
            claims.Add(new Claim("CollegeId", collegeId.Value.ToString()));

        if (courseId.HasValue)
            claims.Add(new Claim("CourseId", courseId.Value.ToString()));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result) =>
        result.IsSuccess ? new OkResult() : new BadRequestObjectResult(new { Error = (string)result.Error });

    public static IActionResult ToActionResult<T>(this Result<T> result) =>
        result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { Error = (string)result.Error });
}
