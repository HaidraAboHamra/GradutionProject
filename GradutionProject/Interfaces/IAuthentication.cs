using GradutionProject.Abstractions;
using System.Security.Claims;

namespace GradutionProject.Interfaces
{
    public interface IAuthentication
    {
        public Task<Result<ClaimsPrincipal>> Login(string email, string password);
    }
}
