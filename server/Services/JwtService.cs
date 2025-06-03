using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace server.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string GenerateToken(AppUser user, IList<string> roles)
        {
            if (user == null) 
                throw new ArgumentNullException(nameof(user));
            
            if (string.IsNullOrEmpty(user.Id))
                throw new ArgumentException("User ID cannot be null or empty", nameof(user));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64) // Issued at
            };

            // Only add email claim if email is not null or empty
            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            // Only add username claim if username is not null or empty
            if (!string.IsNullOrEmpty(user.UserName))
            {
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            }

            // Add role claims
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    if (!string.IsNullOrEmpty(role))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }
            }

            var key = GetSecurityKey();
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresInMinutes = GetExpirationMinutes();
            var expires = DateTime.UtcNow.AddMinutes(expiresInMinutes);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = GetSecurityKey();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string? GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public string? GetEmailFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst(ClaimTypes.Email)?.Value;
        }

        public IEnumerable<string> GetRolesFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindAll(ClaimTypes.Role)?.Select(c => c.Value) ?? Enumerable.Empty<string>();
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                return jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true; // If we can't read the token, consider it expired
            }
        }

        private SymmetricSecurityKey GetSecurityKey()
        {
            var key = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("JWT Key is not configured");

            if (key.Length < 32)
                throw new InvalidOperationException("JWT Key must be at least 32 characters long");

            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        private double GetExpirationMinutes()
        {
            var expiresInMinutesConfig = _config["Jwt:ExpiresInMinutes"];
            if (string.IsNullOrEmpty(expiresInMinutesConfig))
                return 60; // Default to 60 minutes

            if (double.TryParse(expiresInMinutesConfig, out double minutes))
                return minutes;

            return 60; // Default to 60 minutes if parsing fails
        }
    }
}