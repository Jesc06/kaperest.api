using KapeRest.Application.Interfaces.Auth;
using KapeRest.Application.Interfaces.Admin.PendingAcc;
using KapeRest.Application.Interfaces.CurrentUserService;
using KapeRest.Application.Interfaces.Jwt;
using KapeRest.Application.Services.Auth;
using KapeRest.Application.Services.Admin.PendingAcc;
using KapeRest.Infrastructures.Persistence.Database;
using KapeRest.Infrastructures.Persistence.Repositories.Account;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.PendingAccounts;
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
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Application.Services.Admin.Inventory;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Supplier;
using KapeRest.Application.Services.Admin.Supplier;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.Suppliers;
using DotNetEnv;
using KapeRest.Application.Interfaces.Admin.CreateMenuItem;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.CreateMenuItem;
using KapeRest.Application.Services.Admin.CreateMenuItem;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

Env.Load();

#region --Identity--
var connectionString = Environment.GetEnvironmentVariable("KapeRest_DB");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentity<Users, IdentityRole>(options =>
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

#region --Dependency Injection--
builder.Services.AddScoped<IAccounts, RegisterAccountRepositories>();
builder.Services.AddScoped<AccountService>();

builder.Services.AddScoped<IJwtService, GenerateTokenService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUserService>();

builder.Services.AddScoped<IpendingAccount, PendingAccountRepo>();
builder.Services.AddScoped<PendingAccService>();

builder.Services.AddScoped<IInventory, AddProductRepo>();
builder.Services.AddScoped<AddProductService>();

builder.Services.AddScoped<ISupplier, AddSupplierRepo>();
builder.Services.AddScoped<AddSupplierService>();

builder.Services.AddScoped<IMenuItem, MenuItemRepo>();
builder.Services.AddScoped<MenuItemService>();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

#region --Seed Admin Account--
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    await AdminSeededAccount.AdminAccount(roleManager, userManager, config);
}
#endregion

app.Run();
