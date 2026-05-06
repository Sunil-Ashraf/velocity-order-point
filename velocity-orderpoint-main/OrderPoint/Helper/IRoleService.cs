using System.Security.Claims;
using System.Threading.Tasks;

public interface IRoleService
{
    Task<bool> IsInRoleAsync(string roleName);
    Task<ClaimsPrincipal> GetCurrentUserAsync();
}