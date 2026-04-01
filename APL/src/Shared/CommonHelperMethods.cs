using System.Security.Claims;

namespace APL.Shared
{
    public class CommonHelperMethods
    {
        public static string? GetPreferredUsername(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
                return null;

            return user.Claims
                       .FirstOrDefault(c => c.Type == "preferred_username")
                       ?.Value;
        }
    }
}
