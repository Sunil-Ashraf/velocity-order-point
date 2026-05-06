using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Security.Claims;
using OrderPoint.Helper;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OrderPoint.Domain.ViewModel;
using System.Net.Http;
using System.Text.Json;
using OrderPoint.Domain.Common;

namespace OrderPoint.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly NavigationManager _navigationManager;
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _Accessor;

        private readonly ILocalStorageService _localStorage;

        private bool _isPrerendering = true; // Initially assume we are prerendering

        public JwtAuthenticationStateProvider(IJSRuntime jsRuntime, ILocalStorageService localStorage, NavigationManager navigationManager, HttpClient httpClient, IConfiguration configuration , IHttpContextAccessor accessor)
        {
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
            _httpClient = httpClient;
            _localStorage = localStorage;
            _configuration = configuration;
            _Accessor = accessor;
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


            var context = _Accessor.HttpContext;
            var wholesalerModel = context?.Session.GetObject<AddEditWholesalerModel>("Wholesaler");

            //var claimwholesalerID = await  GetClaimValue("WholesalerID");
            var token = await _localStorage.GetItemAsync<string>("authToken")
                      ?? await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");

            var claimwholesalerID = await GetClaimValue("WholesalerID");

            //if (wholesalerModel!= null&& !String.IsNullOrEmpty(claimwholesalerID) && Convert.ToInt32(claimwholesalerID) != wholesalerModel.ID)
            //{
            //    return false;
            //}

            if (string.IsNullOrEmpty(token))
                return false;

            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jwt;
            try
            {
                jwt = handler.ReadJwtToken(token);

                var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType");
                var user = new ClaimsPrincipal(identity);
                string userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var apiUrl = $"{_configuration["API_App_URL"]}/api/Account/LoginWithUserId?userId={userId}";
                var response = await _httpClient.GetAsync(apiUrl);


                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<APIResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (result.Success)
                    {
                        return true;
                    }
                    else
                    {
                        return false;

                    }
                }

            }
            catch
            {
                return false;
            }
            var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (expClaim == null) return false;
            var exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;
            return exp > DateTime.UtcNow;  
        }

         
    }

}
