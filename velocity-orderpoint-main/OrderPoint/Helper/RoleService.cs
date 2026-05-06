using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using OrderPoint.Domain.Common;
using System.Security.Claims;


 

// Services/RoleService.cs

using System.Threading.Tasks;
namespace OrderPoint.Helper
{
    public class RoleService : IRoleService
    {
        private readonly AuthenticationStateProvider _authProvider;
        private readonly ILocalStorageService _localStorage;

        private readonly IJSRuntime _jsRuntime;
 
     
        public RoleService(AuthenticationStateProvider authProvider, ILocalStorageService localStorage, IJSRuntime jsRuntime)
        {
            _authProvider = authProvider;
            _localStorage = localStorage;
            _jsRuntime = jsRuntime;
        }

        public async Task<ClaimsPrincipal> GetCurrentUserAsync()
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            return authState.User;
        }

        public async Task<bool> IsInRoleAsync(string roleName)
        {
            // Optional: Prevent JS calls during prerendering
            if (_jsRuntime is not IJSInProcessRuntime)
            {
                return false; // or throw or log
            }

            var roles = await GetClaimValues(ClaimTypes.Role);
            return roles.Contains(roleName);
        }

        public async Task<List<string>> GetClaimValues(string claimType)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            var identity = string.IsNullOrEmpty(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType");

            return identity.Claims
                           .Where(c => c.Type == claimType)
                           .Select(c => c.Value)
                           .ToList();
        }
        public async Task<List<string>> GetRolesAsync()
        {

            var user = await GetCurrentUserAsync();

            if (user == null || !user.Identity.IsAuthenticated)
                return new List<string>();
            

            return user.Claims
                       .Where(c => c.Type == ClaimTypes.Role)
                       .Select(c => c.Value)
                       .ToList();
        }
    }
}