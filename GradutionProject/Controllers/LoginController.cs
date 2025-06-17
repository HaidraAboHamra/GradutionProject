using GradutionProject.Abstractions;
using GradutionProject.Data;
using GradutionProject.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GradutionProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public LoginController(UserService userService, IConfiguration configuration,ApplicationDbContext applicationDbContext)
        {
            _userService = userService;
            _configuration = configuration;
            _db = applicationDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Error = "Invalid request" });

            var email = request.Email.ToLowerInvariant();

            Result<object> loginResult;

            if (email.Contains("admin"))
                loginResult = await HandleAdminLoginAsync(email, request.Password);
            else if (email.Contains("prof"))
                loginResult = await HandleLoginAsync(email, request.Password, "Professor");
            else
                loginResult = await HandleLoginAsync(email, request.Password, "User");

            return loginResult.ToActionResult();
        }

        #region Login Handlers

        private async Task<Result<object>> HandleLoginAsync(string email, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return Result.Failure<object>(new Error("Email and password are required"));

            var user = await _userService.LoginAsync(email, password);
            if (user == null)
                return Result.Failure<object>(new Error("Invalid email or password"));

            var token = GenerateJwtToken(
                userId: user.Id.ToString(),
                email: user.Email,
                phone: user.PhoneNumber,
                role: role,
                collegeId: user.CollegeId
            );

            return Result.Success<object>(new
            {
                Message = "Login successful",
                UserId = user.Id,
                Role = role,
                Token = token
            });
        }

        private async Task<Result<object>> HandleAdminLoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return Result.Failure<object>(new Error("Email and password are required"));

            var admin = await _userService.LoginAdminAsync(email, password);
            if (admin == null)
                return Result.Failure<object>(new Error("Invalid email or password"));
            var collegeId =  _db.Colleges.FirstOrDefault(x=> x.AdminId == admin.Id);
            var token = GenerateJwtToken(
                userId: admin.Id.ToString(),
                email: admin.Email,
                phone: admin.Phone,
                role: "Admin",
                collegeId: collegeId?.Id
            );

            return Result.Success<object>(new
            {
                Message = "Login successful",
                UserId = admin.Id,
                Role = "Admin",
                Token = token
            });
        }

        #endregion

        #region JWT Generator

        private string GenerateJwtToken(string userId, string email, string? phone, string role, int? collegeId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.MobilePhone, phone ?? string.Empty),
                new Claim(ClaimTypes.Role, role)
            };

            if (collegeId.HasValue)
                claims.Add(new Claim("CollegeId", collegeId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion

        #region Models & Helpers

        public class LoginRequest
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        #endregion
    }

    public static class ResultExtensions
    {
        public static IActionResult ToActionResult(this Result result) =>
            result.IsSuccess
                ? new OkResult()
                : new BadRequestObjectResult(new { Error = result.Error?.ToString() });

        public static IActionResult ToActionResult<T>(this Result<T> result) =>
            result.IsSuccess
                ? new OkObjectResult(result.Value)
                : new BadRequestObjectResult(new { Error = result.Error?.ToString() });
    }
}
