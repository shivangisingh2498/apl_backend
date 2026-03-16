using APL.Data;
using APL.Middleware;
using APL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Database Configuration ---
builder.Services.AddDbContext<AplDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. Dependency Injection Services ---
builder.Services.AddControllers();
builder.Services.AddScoped<IFormMasterService, FormMasterService>();
builder.Services.AddScoped<IBscTemplateService, BscTemplateService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

// --- 3. Authentication & JWT Configuration ---
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.Events = new JwtBearerEvents
//        {
//            OnMessageReceived = context =>
//            {
//                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
//                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
//                    return Task.CompletedTask;

//                var token = authHeader.Substring("Bearer ".Length).Trim();
//                var handler = new JwtSecurityTokenHandler();

//                if (handler.CanReadToken(token))
//                {
//                    var jwt = handler.ReadJwtToken(token);

//                    // Manually create the Identity. 
//                    // "DummyAuth" is the authentication type (marks it as Authenticated)
//                    // "name" and "roles" match your JSON keys
//                    var identity = new ClaimsIdentity(jwt.Claims, "DummyAuth", "name", "roles");

//                    context.Principal = new ClaimsPrincipal(identity);
//                    context.Success(); // This skips the internal signature check
//                }
//                return Task.CompletedTask;
//            }
//        };

//        // These parameters are still needed as a fallback, but OnMessageReceived takes priority
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = false,
//            RequireSignedTokens = false,
//            NameClaimType = "name",
//            RoleClaimType = "roles"
//        };
//    });

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
// --- 4. Middleware Pipeline ---
// The order here is critical for security to function
app.UseHttpsRedirection();

app.UseAuthentication(); // 1st: Identify who the user is
app.UseAuthorization();  // 2nd: Check what the user is allowed to do

app.MapControllers();

// --- 5. Debug Endpoint ---
// Use this to verify your claims: http://localhost:PORT/debug/me
app.MapGet("/debug/me", (ClaimsPrincipal user) =>
{
    return Results.Ok(new
    {
        isAuthenticated = user.Identity?.IsAuthenticated ?? false,
        userName = user.Identity?.Name,
        roles = user.Claims.Where(c => c.Type == "roles").Select(c => c.Value).ToList(),
        allClaims = user.Claims.Select(c => new { c.Type, c.Value })
    });
}).RequireAuthorization();

app.Run();