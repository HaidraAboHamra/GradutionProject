using GradutionProject.Abstractions;
using GradutionProject.Interfaces;
using System.Security.Claims;

namespace GradutionProject.Repositories;

public class AuthenticationRepository(UserService userService) : IAuthentication
{
    public async Task<Result<ClaimsPrincipal>> Login(string email, string password)
    {
        var result = await userService.LoginAsync(email, password);
        if (result is null)
        {
            return Result<ClaimsPrincipal>.Failure(new Error("Login failed"));
        }

        var role = result.IsAdmin ? "Admin" : "User";
        var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, $"{result.Name}"),
                        new("Id", $"{result.Id}"),
                        new(ClaimTypes.Role, role)
                    };
        var identity = new ClaimsIdentity(claims, "AuthenticationType");
        var user = new ClaimsPrincipal(identity);
        return Result<ClaimsPrincipal>.Success(user);
    }
}
