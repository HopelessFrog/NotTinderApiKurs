using AuthApi.Models.DTOs;

namespace JwtAuthentication;

public interface ITokenGenerator
{
    Task<TokenDto> GenerateToken(CreateTokenDTO request);
}