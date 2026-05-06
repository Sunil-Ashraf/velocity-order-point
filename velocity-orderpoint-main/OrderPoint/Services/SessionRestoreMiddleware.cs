using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using OrderPoint.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Blazored.LocalStorage;
using OrderPoint.Components.Common;
using System.Text.Json;
using OrderPoint.Domain.ViewModel;
using OrderPoint.Helper;
using System.IdentityModel.Tokens.Jwt; // Example namespace where your DB service exists
namespace OrderPoint.Services
{
    public class SessionRestoreMiddleware
    {
        private readonly RequestDelegate _next;
        //   private readonly ILocalStorageService _localStorage;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        //private readonly JwtAuthenticationStateProvider _authProvider;

         //private readonly IServiceScopeFactory _scopeFactory;

        public SessionRestoreMiddleware(RequestDelegate next, IConfiguration configuration, HttpClient httpClient)
        {
            _next = next;
            // _localStorage = localStorage;
            _configuration = configuration;
            _httpClient = httpClient;
            //_authProvider = authProvider;

            //_scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)

        {
            // Skip Blazor internal and static requests
            var path = context.Request.Path;
            if (path.StartsWithSegments("/_blazor") ||
                path.StartsWithSegments("/_framework") ||
                path.StartsWithSegments("/_content") ||
                  path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            var session = context.Session;
            var userId = session.GetString("UserId");
            var url = context.Request.GetDisplayUrl();
            string name = context.Request.GetDisplayUrl().Split('/')[context.Request.GetDisplayUrl().Split('/').Length - 1];

            //var handler = new JwtSecurityTokenHandler();
            //var tokenFromHeader = context.Request.Headers["Authorization"].ToString();
            ////var token = await _authProvider.GetAuthToken();

            //using var scope = _scopeFactory.CreateScope();
            //var auth = scope.ServiceProvider.GetRequiredService<JwtAuthenticationStateProvider>();
            //var claimwholesalerID = await auth.GetAuthToken();
            // var claimwholesalerID = await _AuthProvider.GetClaimValue("WholesalerID");
            //Int32 clamWsalerID;

            //   string   WholesaleLogo = await _localStorage.GetItemAsync<string>("WholesalerLogo");
            var wholesalerID = context.Session.GetString("WholesalerID");

            //using var scope = _scopeFactory.CreateScope();

           // var claimwholesalerID = context.User?.FindFirst("WholesalerID")?.Value;

           
            //if (!String.IsNullOrEmpty(claimwholesalerID) && claimwholesalerID != wholesalerID)
            //{
            //    if (context.Request.Cookies.ContainsKey("authToken"))
            //    {
            //        context.Response.Cookies.Delete("authToken");
            //    }
            //}

            if (url.Contains("Home"))
            {
                if (string.IsNullOrEmpty(wholesalerID))
                {

                    if (!string.IsNullOrEmpty(name))
                    {


                        var apiUrl = $"{_configuration["API_App_URL"]}/api/Wholesaler/GetWholesalerByName?name={name}";
                        var response = await _httpClient.GetAsync(apiUrl);


                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var result = JsonSerializer.Deserialize<OrderPoint.Domain.Common.Response<AddEditWholesalerModel>>(jsonString,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                            var wholesaler = result?.Data;
                            if (wholesaler != null)
                            {
                                context.Session.SetObject("Wholesaler", wholesaler);
                                context.Session.SetString("WholesalerID", wholesaler.ID.ToString() ?? "");
                            }
                            else
                            {
                                var wholesalers = new AddEditWholesalerModel();
                                context.Session.SetObject("Wholesaler", wholesalers);
                                context.Session.SetString("WholesalerID", wholesalers.ID?.ToString() ?? "");
                            }
                        }
                        else
                        {
                            var wholesalers = new AddEditWholesalerModel();
                            context.Session.SetObject("Wholesaler", wholesalers);
                            context.Session.SetString("WholesalerID", wholesalers.ID?.ToString() ?? "");
                        }
                    }

                }
                else
                {
                    var wholesalerModel = context.Session.GetObject<AddEditWholesalerModel>("Wholesaler");
                    if (wholesalerModel != null && wholesalerModel.Name != name)
                    {
                        var apiUrl = $"{_configuration["API_App_URL"]}/api/Wholesaler/GetWholesalerByName?name={name}";
                        var response = await _httpClient.GetAsync(apiUrl);


                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var result = JsonSerializer.Deserialize<OrderPoint.Domain.Common.Response<AddEditWholesalerModel>>(jsonString,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                            var wholesaler = result?.Data;
                            if (wholesaler != null)
                            {
                                context.Session.SetObject("Wholesaler", wholesaler);
                                context.Session.SetString("WholesalerID", wholesaler.ID.ToString() ?? "");
                            }
                            else
                            {
                                var wholesalers = new AddEditWholesalerModel();
                                context.Session.SetObject("Wholesaler", wholesalers);
                                context.Session.SetString("WholesalerID", wholesalers.ID?.ToString() ?? "");
                            }
                        }
                        else
                        {
                            var wholesalers = new AddEditWholesalerModel();
                            context.Session.SetObject("Wholesaler", wholesalers);
                            context.Session.SetString("WholesalerID", wholesalers.ID?.ToString() ?? "");
                        }
                    }

                }
            }

            await _next(context);
        }
    }
}
