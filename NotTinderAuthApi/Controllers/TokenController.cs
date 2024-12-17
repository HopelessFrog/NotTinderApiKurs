using AuthApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using TokenServices;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly IRefreshTokenService _refreshTokenService;

    public TokenController(IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost]
    public async Task<IActionResult> Refresh([FromBody] TokenDto data)
    {
        try
        {
            var tokenDtoToReturn = await
                _refreshTokenService.RefreshTokenAsync(data);
            return Ok(tokenDtoToReturn);
        }
        catch (Exception e)
        {
            return Unauthorized();
        }
    }
}