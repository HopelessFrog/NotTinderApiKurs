using AuthApi.Models.DTOs;

namespace TokenServices;

public interface IRefreshTokenService
{
    Task<TokenDto> RefreshTokenAsync(TokenDto data);
}