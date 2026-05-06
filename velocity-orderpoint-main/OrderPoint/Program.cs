using Blazored.LocalStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using OrderPoint.Components;
using OrderPoint.Helper;
using OrderPoint.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Globalization;
using MudBlazor.Services;
using Microsoft.AspNetCore.SignalR.Client;
var builder = WebApplication.CreateBuilder(args);
// Add JWT Authentication
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
// 1. Authentication and Authorization
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<JwtAuthenticationStateProvider>());
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    //options.Authority = "https://localhost:7111"; // External auth provider URL
    options.Authority = "https://digin.startech.pk"; // External auth provider URL
    options.Audience = builder.Configuration["API_App_URL"];  // The API audience you set up in the auth provider

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});
// ===== 2. SignalR HubConnection =====
// Register HubConnection as scoped service for Blazor WASM
builder.Services.AddScoped(sp =>
{
    return new HubConnectionBuilder()
        .WithUrl(builder.Configuration["API_App_URL"] + "/userHub", options =>
        {
            // Pass JWT token to SignalR hub
            options.AccessTokenProvider = async () =>
            {
                var localStorage = sp.GetRequiredService<ILocalStorageService>();
                return await localStorage.GetItemAsync<string>("authToken");
            };
        })
        .WithAutomaticReconnect()
        .Build();
});
builder.Services.AddAuthorizationCore();

 
builder.Services.AddHttpClient();
builder.Services.AddBlazoredToast();
builder.Services.AddBlazorBootstrap();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<CookieService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<LogoutService>();
builder.Services.AddMudServices();
builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(90);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

//var ukCulture = new CultureInfo("en-GB");
//CultureInfo.DefaultThreadCurrentCulture = ukCulture;
//CultureInfo.DefaultThreadCurrentUICulture = ukCulture;
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // Authentication middleware
app.UseAuthorization(); // Authorization middleware
app.UseStaticFiles();
app.UseAntiforgery();
app.UseSession();
app.UseMiddleware<SessionRestoreMiddleware>();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
