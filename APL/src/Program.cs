using APL.Data;
using APL.Middleware;
using APL.Services;
using APL.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Database Configuration ---

// Scoped DbContext for normal usage
builder.Services.AddDbContext<AplDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
},
contextLifetime: ServiceLifetime.Scoped,
optionsLifetime: ServiceLifetime.Singleton);

// Factory for parallel operations
builder.Services.AddDbContextFactory<AplDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddMemoryCache();
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

// --- 2. Dependency Injection Services ---
builder.Services.AddControllers();
builder.Services.AddScoped<IBscTemplateService, BscTemplateService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IMasterDataService, MasterDataService>();
builder.Services.AddScoped<ITargetSettingsService, TargetSettingsService>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// 1. Setup a "Development" Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // During development, we disable strict validation since we don't have Azure keys
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = false,
        // This allows us to use any "Fake" token for testing
        SignatureValidator = delegate (string token, TokenValidationParameters parameters)
        {
            return new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token);
        }
    };
});

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
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
app.UseCors("AllowAngularApp");


// --- 4. Middleware Pipeline ---
// The order here is critical for security to function
app.UseHttpsRedirection();

app.UseAuthentication(); // 1st: Identify who the user is
app.UseAuthorization();  // 2nd: Check what the user is allowed to do

app.MapControllers();
app.Run();