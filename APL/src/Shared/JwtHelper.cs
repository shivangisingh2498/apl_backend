namespace APL.Shared
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Tokens;

    public static class JwtHelper
    {
        /// <summary>
        /// Validates issuer/audience and then extracts roles.
        /// For local dev you can set ValidateIssuerSigningKey=false to skip signature validation.
        /// In production, configure IssuerSigningKeys with Azure AD JWKS and set ValidateIssuerSigningKey=true.
        /// </summary>
        public static (IReadOnlyList<string> Roles, IReadOnlyList<string> Scopes) ValidateAndGetRoles(
            string jwt,
            string validIssuer,      // e.g., "https://login.microsoftonline.com/{tenantId}/v2.0"
            string validAudience,    // e.g., your API's Application ID URI or clientId
            bool validateSignature = false,
            IEnumerable<SecurityKey>? signingKeys = null)
        {
            var handler = new JwtSecurityTokenHandler();

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = validIssuer,

                ValidateAudience = true,
                ValidAudience = validAudience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5),

                ValidateIssuerSigningKey = validateSignature,
                IssuerSigningKeys = validateSignature ? (signingKeys ?? Enumerable.Empty<SecurityKey>()) : null
            };

            // If skipping signature validation, allow tokens without a key
            if (!validateSignature)
            {
                parameters.SignatureValidator = (token, _) =>
                {
                    // Just parse the token into a JwtSecurityToken for claims extraction
                    return handler.ReadJwtToken(token);
                };
            }

            handler.ValidateToken(jwt, parameters, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            var roles = jwtToken.Claims
                .Where(c => c.Type.Equals("roles", StringComparison.OrdinalIgnoreCase) ||
                            c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value)
                .ToList();

            // If roles value is a JSON array in a single claim, split it
            // (simple heuristic; for robust handling use the NoValidate helper above)
            roles = roles.SelectMany(v =>
            {
                if (v.TrimStart().StartsWith("["))
                {
                    try
                    {
                        var arr = System.Text.Json.JsonSerializer.Deserialize<string[]>(v);
                        return arr ?? Array.Empty<string>();
                    }
                    catch { return new[] { v }; }
                }
                return new[] { v };
            })
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

            var scopes = new List<string>();
            var scp = jwtToken.Claims.FirstOrDefault(c => c.Type == "scp")?.Value;
            if (!string.IsNullOrWhiteSpace(scp))
            {
                scopes.AddRange(scp.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            }

            return (roles, scopes);
        }
    }

}
