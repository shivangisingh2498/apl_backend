using APL.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace APL.Shared
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _cache;

        public ClaimsTransformation(IServiceProvider serviceProvider, IMemoryCache cache)
        {
            _serviceProvider = serviceProvider;
            _cache = cache;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var email = principal.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            if (string.IsNullOrEmpty(email)) return principal;

            // Get token expiration to sync cache
            var expClaim = principal.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            var expiryTime = expClaim != null
                ? DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim))
                : DateTimeOffset.UtcNow.AddMinutes(20);

            string cacheKey = $"UserRoles_{email}";

            if (!_cache.TryGetValue(cacheKey, out List<string> roles))
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AplDbContext>();

                roles = await db.tbl_user_management
                    .Include(x => x.tbl_roles_master)
                    .Where(x => x.email == email && x.isactive && !x.isdisable)
                    .Select(x => x.tbl_roles_master.roles)
                    .ToListAsync();

                // Set cache to expire when the token expires
                _cache.Set(cacheKey, roles, expiryTime);
            }

            // Add claims to the principal
            var id = new ClaimsIdentity(principal.Identity);
            foreach (var role in roles)
            {
                id.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return new ClaimsPrincipal(id);
        }
    }
}
