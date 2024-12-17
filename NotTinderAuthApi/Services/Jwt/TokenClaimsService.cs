using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Entities;
using JwtAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TokenShared;

namespace TokenServices;

public class TokenClaimsService : ITokenClaimsService
{
    private readonly JwtConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public TokenClaimsService(IOptionsSnapshot<JwtConfiguration> configuration, UserManager<User> userManager,
        ITokenGenerator tokenGenerator)
    {
        _configuration = configuration.Value;
        _userManager = userManager;
    }

    public string GetDeviceIdFromAccessToken(string token)
    {
        var claims = GetClaimsFromToken(token);
        return claims.FindFirstValue(ClaimTypes.SerialNumber);
    }

    public async Task<User> GetUserFromAccessToken(string token)
    {
        var claims = GetClaimsFromToken(token);
        return await _userManager.FindByNameAsync(claims.Identity.Name);
    }


    private ClaimsPrincipal GetClaimsFromToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Key)),
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase)) throw new SecurityTokenException("Invalid token");

        return principal;
    }
}