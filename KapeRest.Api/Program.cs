using DotNetEnv;
using KapeRest.Application.Interfaces.Admin.Branch;
using KapeRest.Application.Interfaces.Admin.CreateMenuItem;
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.PendingAcc;
using KapeRest.Application.Interfaces.Admin.Supplier;
using KapeRest.Application.Interfaces.Auth;
using KapeRest.Application.Interfaces.Cashiers.Buy;
using KapeRest.Application.Interfaces.Cashiers.Sales;
using KapeRest.Application.Interfaces.CurrentUserService;
using KapeRest.Application.Interfaces.PayMongo;
using KapeRest.Application.Services.Admin.Branch;
using KapeRest.Application.Services.Admin.CreateMenuItem;
using KapeRest.Application.Services.Admin.Inventory;
using KapeRest.Application.Services.Admin.PendingAcc;
using KapeRest.Application.Services.Admin.Supplier;
using KapeRest.Application.Services.Auth;
using KapeRest.Application.Services.Cashiers.Buy;
using KapeRest.Application.Services.Cashiers.Sales;
using KapeRest.Infrastructure.DependencyInjection;
using KapeRest.Infrastructure.Persistence.Repositories.Admin.Branch;
using KapeRest.Infrastructure.Persistence.Repositories.Cashiers.Sales;
using KapeRest.Infrastructure.Services.PayMongoService;
using KapeRest.Infrastructures.Persistence.Database;
using KapeRest.Infrastructures.Persistence.Repositories.Account;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.CreateMenuItem;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.Inventory;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.PendingAccounts;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.Suppliers;
using KapeRest.Infrastructures.Persistence.Repositories.Cashiers.Buy;
using KapeRest.Infrastructures.Persistence.Seeder;
using KapeRest.Infrastructures.Services.CurrentUserService;
using KapeRest.Infrastructures.Services.JwtService; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
     .AddJsonOptions(opt =>
     {
         opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
         opt.JsonSerializerOptions.WriteIndented = true;
     });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSevices();

Env.Load();

#region--PayMongo Secret Key Injection--
    var apiKey = Environment.GetEnvironmentVariable("ApiKey");
    if (string.IsNullOrEmpty(apiKey))
        throw new Exception("PayMongo API key not found in .env");

    builder.Services.AddScoped<IPayMongo>(provider => new PayMongo(apiKey));
#endregion

#region --Identity--
var connectionString = Environment.GetEnvironmentVariable("KapeRest_DB");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddIdentity<UsersIdentity, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.User.RequireUniqueEmail = false;
    options.SignIn.RequireConfirmedEmail = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>();
#endregion

#region --Token Autorization UI in Swagger--
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description =  "Enter your JWT Access Token",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});
#endregion

#region --JWT Authentication--
var Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
var JwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
var key = Encoding.UTF8.GetBytes(JwtKey!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            RoleClaimType = ClaimTypes.Role,

            ClockSkew = TimeSpan.Zero
        };

    });
builder.Services.AddAuthorization();
#endregion

#region--CORS React.JS Client--
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:3001",
            "http://localhost:5173",
            "http://localhost:3002",
            "http://localhost:3004"

        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

    app.UseCors("AllowReactApp");

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

#region --Seed Admin Account--
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UsersIdentity>>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    await AdminSeededAccount.AdminAccount(roleManager, userManager, config);
}
#endregion

app.Run();
