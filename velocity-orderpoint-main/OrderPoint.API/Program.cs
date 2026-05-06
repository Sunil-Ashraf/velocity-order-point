using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderPoint.API.Hubs;
using OrderPoint.Application.Interfaces.Customers;
using OrderPoint.Application.Interfaces.Email;
using OrderPoint.Application.Interfaces.Product;
using OrderPoint.Application.Interfaces.User;
using OrderPoint.Application.Services.Customers;
using OrderPoint.Application.Services.Email;
using OrderPoint.Application.Services.EmailTemplates;
using OrderPoint.Application.Services.Product;
using OrderPoint.Application.Services.User;
using OrderPoint.Domain.DbContexts;
using OrderPoint.Domain.DbContexts.Repositories;
using OrderPoint.Domain.Helper;
using OrderPoint.Domain.Interfaces.Customers;
using OrderPoint.Domain.Interfaces.Product;
using OrderPoint.Domain.Interfaces.User;
using OrderPoint.Domain.ModifyIdentity;
using OrderPoint.Persistence.Repositories.Account;
using OrderPoint.Persistence.Repositories.Customers;
using OrderPoint.Persistence.Repositories.Product;
using System.Text;

string allowOrigins = "_AllowOrigins";
var builder = WebApplication.CreateBuilder(args);
var origin = builder.Configuration["Web_App_URL"] ?? "https://localhost:5001"; 

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("OrderPoint.Persistence")
    ));
// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add services to the container.
builder.Services.AddScoped(typeof(IRepository<>));

builder.Services.AddSignalR();
// ====== CORS ======
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowOrigins, policy =>
    {
        policy
            .WithOrigins(origin) // e.g. https://portal.yoursite.com
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();  //  REQUIRED for SignalR WebSockets
        // remove AllowCredentials unless you specifically need cookies/auth
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddHttpClient();
//builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Identity options (password requirements etc.)
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserHelper>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserAppService, UserAppService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductAppService, ProductAppService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerAppService, CustomersAppService>();

builder.Services.AddScoped<IWholesalerRepository, WholesalerRepository>();
builder.Services.AddScoped<IWholesalerAppService, WholesalerAppService>();

builder.Services.AddScoped<IEmailTemplatesRepository, EmailTemplatesRepository>();
builder.Services.AddScoped<IEmailTemplatesAppService, EmailTemplatesAppService>();

builder.Services.AddScoped<IEmailAppService, EmailAppService>();

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 

app.UseHttpsRedirection();

app.UseAuthentication(); // Enables authentication middleware (JWT, etc.)
app.UseAuthorization();
// Add this line
app.UseStaticFiles();
app.UseCors(allowOrigins);
app.MapControllers();
app.MapHub<UserHub>("/userHub");

//app.UseHttpsRedirection();
app.Run();
