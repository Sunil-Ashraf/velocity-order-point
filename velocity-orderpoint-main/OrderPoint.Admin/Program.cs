using Blazored.LocalStorage;
using Blazored.Toast;
using OrderPoint.Admin.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static Azure.Core.HttpHeader;
using OrderPoint.Admin.Services;
using OrderPoint.Admin.Helper;
using MudBlazor.Services;
var builder = WebApplication.CreateBuilder(args);
// 1. Authentication and Authorization
//builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
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
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<NotificationService>();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// 3. Data and Storage
builder.Services.AddHttpClient();
builder.Services.AddBlazoredToast();
builder.Services.AddBlazorBootstrap();
builder.Services.AddMudServices();

// Add services to the container.

var app = builder.Build();

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
