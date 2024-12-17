using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthApi.Models.DTOs;
using AuthApi.Models.Entities;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TokenShared;

namespace JwtAuthentication;

public class TokenGenerator : ITokenGenerator
{
    private readonly JwtConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public TokenGenerator(IOptionsSnapshot<JwtConfiguration> configuration, UserManager<User> userManager)
    {
        _configuration = configuration.Value;
        _userManager = userManager;
    }

    public async Task<TokenDto> GenerateToken(CreateTokenDTO request)
    {
        var user = await _userManager.FindByNameAsync(request.Name);

        if (user == null)
            throw new Exception("User is null");

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Hash, user.Id),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.SerialNumber, request.DeviceId)
        };

        if (roles != null)
            foreach (var roleClaim in roles)
                claims.Add(new Claim(ClaimTypes.Role, roleClaim));

        var refreshToken = GenerateRefreshToken();
        var authDevice = user?.AuthenticatedDevices.Find(d => d?.DeviceId == request.DeviceId);

        if (authDevice is null)
        {
            user.AuthenticatedDevices.Add(new AuthenticatedDevice
            {
                DeviceId = request.DeviceId, RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.Now.AddDays(7)
            });
        }
        else
        {
            authDevice.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            authDevice.RefreshToken = refreshToken;
        }

        await _userManager.UpdateAsync(user);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Key));

        var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken securityTokenOptions;

        securityTokenOptions = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: signingCred);


        var accessToken = new JwtSecurityTokenHandler().WriteToken(securityTokenOptions);
        return new TokenDto(accessToken, refreshToken);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rnd = RandomNumberGenerator.Create())
        {
            rnd.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}