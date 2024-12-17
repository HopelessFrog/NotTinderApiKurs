using System.Security.Claims;
using AuthApi.Models.DTOs;
using AuthApi.Models.Requests;
using AuthApi.Services.Interfaces;
using JwtAuthentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TokenServices;

namespace AuthApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenClaimsService _tokenClaimsService;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginController(IAuthService authService, ITokenGenerator tokenGenerator,
        ITokenClaimsService tokenClaimsService)
    {
        _authService = authService;
        _tokenGenerator = tokenGenerator;
        _tokenClaimsService = tokenClaimsService;
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] AuthRequest request)
    {
        if (await _authService.Login(request))
        {
            var tokens = await _tokenGenerator.GenerateToken(new CreateTokenDTO
            {
                Name = request.Login,
                DeviceId = request.DeviceId
            });

            var user = await _authService.GetUser(request.Login);
            var loginDto = new LoginDto
            {
                Id = user.Id,
                Name = request.Login,
                Tokens = tokens
            };
            if (user.Roles.Count != 0) loginDto.Roles = user.Roles;

            return Ok(loginDto);
        }

        return Unauthorized("Invalid password or username");
    }

    [HttpPost("LogOut")]
    public async Task<ActionResult> LogOut()
    {
        _authService.LogOut(User.Identity.Name,
            User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value);

        return Ok();
    }
}