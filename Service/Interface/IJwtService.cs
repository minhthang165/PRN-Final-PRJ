using System.Security.Claims;

namespace PRN_Final_Project.Service.Interface
{
    public interface IJwtService
    {
        string GenerateToken(IEnumerable<Claim> claims);
        ClaimsPrincipal? ValidateToken(string token);
    }
}