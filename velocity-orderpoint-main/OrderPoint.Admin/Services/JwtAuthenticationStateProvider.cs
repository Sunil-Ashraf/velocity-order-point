using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Security.Claims;
using OrderPoint.Admin.Helper;
using System.IdentityModel.Tokens.Jwt;

namespace OrderPoint.Admin.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        //private readonly IJSRuntime _jsRuntime;
        private readonly NavigationManager _navigationManager;
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        private bool _isPrerendering = true; // Initially assume we are prerendering

        public JwtAuthenticationStateProvider(  ILocalStorageService localStorage, NavigationManager navigationManager, HttpClient httpClient)
        {
           // _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
            _httpClient = httpClient;
            _localStorage = localStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Check if we are prerendering
            if (_isPrerendering)
            {
                // Return a default state while prerendering
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Actual logic to get token from local storage
            //var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            var token = await _localStorage.GetItemAsync<string>("authToken");

            var identity = string.IsNullOrEmpty(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType");

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        public async Task InitializeAsync()
        {
            // Detect when we have left the prerendering phase
            _isPrerendering = false;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        public async Task<string> GetClaimValue(string claim)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            var identity = string.IsNullOrEmpty(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType");

            return identity.FindFirst(claim)?.Value;

            //var user = new ClaimsPrincipal(identity);
            //return identity?.Claims.FirstOrDefault(c => c.Type == claim)?.Value;
        }

        public void NotifyUserAuthentication(string token)
        {
            var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType");
            var user = new ClaimsPrincipal(identity);
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }
        public async Task<bool> IsTokenValidAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken"); 
                     //?? await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");

            if (string.IsNullOrEmpty(token))
                return false;

            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jwt;
            try
            {
                jwt = handler.ReadJwtToken(token);
            }
            catch
            {
                return false;
            }

            var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (expClaim == null) return false;

            var exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;

            return exp > DateTime.UtcNow; // ✅ Token is still valid
        }
    }

}
