using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Blazored.LocalStorage;
using System.Security.Claims;
using OrderPoint.Helper;
using OrderPoint.Domain.Entities;
using OrderPoint.Domain.ViewModel;
using System.Text.Json;

namespace OrderPoint.Services
{
    public class LogoutService
    {
        private readonly HubConnection _hubConnection;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigation;
        private readonly IHttpContextAccessor _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public LogoutService(HubConnection hubConnection, ILocalStorageService localStorage, NavigationManager navigation, IHttpContextAccessor context, IConfiguration configuration, HttpClient httpClient)
        {
            _hubConnection = hubConnection;
            _localStorage = localStorage;
            _navigation = navigation;
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task InitializeAsync()
        {
            _hubConnection.On<ForceLogoutModel>("ForceLogout", async data =>
            {
                await LogoutAsync(data.type, data.ID);
            });

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }

        public async Task LogoutAsync(string type, string ID)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (token != null)
            {
                var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType");
                var user = new ClaimsPrincipal(identity);
                string userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string wholesalerID = user.FindFirst("WholesalerID")?.Value;
                if (type == "User" && userId == ID)
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    _navigation.NavigateTo("/login", true);
                }
                else if (type == "WholeSaler" && wholesalerID == ID)
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    _navigation.NavigateTo("/login", true);
                }
                else if (type == "WholeSalerSession")
                {
                    await ResetWholesalerSession();
                }
            }
            else if (type == "WholeSalerSession")
            {
                await ResetWholesalerSession();
            }
        }

        public async Task ResetWholesalerSession()
        {
            var wholesalerID = _context.HttpContext.Session.GetString("WholesalerID");
            var apiUrl = $"{_configuration["API_App_URL"]}/api/Wholesaler/GetWholesalerByID?Id={wholesalerID}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OrderPoint.Domain.Common.Response<AddEditWholesalerModel>>(jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var wholesaler = result?.Data;
                if (wholesaler != null)
                {
                    _context.HttpContext.Session.SetObject("Wholesaler", wholesaler);
                    _context.HttpContext.Session.SetString("WholesalerID", wholesaler.ID.ToString() ?? "");
                    _navigation.NavigateTo(_navigation.Uri, forceLoad: true);
                }
                else
                {
                    var wholesalers = new AddEditWholesalerModel();
                    _context.HttpContext.Session.SetObject("Wholesaler", wholesalers);
                    _context.HttpContext.Session.SetString("WholesalerID", wholesalers.ID?.ToString() ?? "");
                    _navigation.NavigateTo(_navigation.Uri, forceLoad: true);
                }
            }
            else
            {
                var wholesalers = new AddEditWholesalerModel();
                _context.HttpContext.Session.SetObject("Wholesaler", wholesalers);
                _context.HttpContext.Session.SetString("WholesalerID", wholesalers.ID?.ToString() ?? "");
                _navigation.NavigateTo(_navigation.Uri, forceLoad: true);
            }
        }
    }

    public class ForceLogoutModel
    {
        public string type { get; set; }
        public string ID { get; set; }
    }
}