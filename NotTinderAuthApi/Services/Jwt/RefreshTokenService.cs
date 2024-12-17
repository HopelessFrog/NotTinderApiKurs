using AuthApi.Models.DTOs;
using JwtAuthentication;

namespace TokenServices;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ITokenClaimsService _tokenClaimsService;

    private readonly ITokenGenerator _tokenGenerator;

    public RefreshTokenService(ITokenGenerator tokenGenerator, ITokenClaimsService tokenClaimsService)
    {
        _tokenGenerator = tokenGenerator;
        _tokenClaimsService = tokenClaimsService;
    }

    public async Task<TokenDto> RefreshTokenAsync(TokenDto data)
    {
        var user = await _tokenClaimsService.GetUserFromAccessToken(data.AccessToken);
        var deviceId = _tokenClaimsService.GetDeviceIdFromAccessToken(data.AccessToken);

        var device = user.AuthenticatedDevices.FirstOrDefault(d => d.DeviceId.ToString() == deviceId);
        if (user == null || device == null) throw new Exception("Something bad");

        if (device.RefreshToken != data.RefreshToken || device.RefreshTokenExpiryTime <= DateTime.Now)
            throw new Exception("Something bad");


        return await _tokenGenerator.GenerateToken(new CreateTokenDTO { Name = user.UserName, DeviceId = deviceId });
    }
}