using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BuildingBlocks.Configurations;
using Cinema.Gateway.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region DataBase

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

#endregion

#region Auth

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());
    options.AddPolicy("AllowAnonymous", policy =>
        policy.RequireAssertion(_ => true));
});

#endregion

#region Logging

builder.AddSerilogWithOpenTelemetry();

#endregion

#region OpenTelemetry

builder.Logging.AddOpenTelemetryLogging();
builder.Services.AddOpenTelemetryMetricsAndTracing(builder.Environment.ApplicationName);

#endregion

#region Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

#region Gateway

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

#endregion



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MigrateDatabase();
}

#region Middlewares

app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();

#endregion

#region Routes

app.MapPost("/user/register", async (UserManager<IdentityUser> userManager, RegisterRequest request) =>
{
    var user = new IdentityUser { UserName = request.Email, Email = request.Email };
    var result = await userManager.CreateAsync(user, request.Password);

    if (!result.Succeeded)
        return Results.BadRequest(result.Errors);

    return Results.Ok("User created successfully");
});

app.MapPost("/user/login",
    async (UserManager<IdentityUser> userManager, IConfiguration configuration, LoginRequest request) =>
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Results.Unauthorized();

        var token = GenerateJwtToken(user, configuration);
        return Results.Ok(new { Token = token });
    });

#endregion

#region JWT

string GenerateJwtToken(IdentityUser user, IConfiguration configuration)
{
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        configuration["Jwt:Issuer"],
        configuration["Jwt:Audience"],
        claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

#endregion

app.Run();

public record RegisterRequest(string Email, string Password);

public record LoginRequest(string Email, string Password);