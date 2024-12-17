using AuthApi.Models.DTOs;
using AuthApi.Models.Requests;
using AuthApi.Services.Interfaces;
using Entities;
using Microsoft.AspNetCore.Identity;
using TokenServices;

namespace AuthApi.Services;

public class AuthService : IAuthService
{
    private readonly ITokenClaimsService _tokenClaimsService;
    private readonly UserManager<User> _userManager;

    public AuthService(UserManager<User> userManager, ITokenClaimsService tokenClaimsService)
    {
        _userManager = userManager;
        _tokenClaimsService = tokenClaimsService;
    }

    public async Task<UserDto> GetUser(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) throw new Exception("User not found");
        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Name = user.UserName,
            Roles = roles.ToList()
        };
    }


    public async Task<bool> Login(AuthRequest request)
    {
        var identityUser = await _userManager.FindByNameAsync(request.Login);
        if (identityUser is null) return false;


        return await _userManager.CheckPasswordAsync(identityUser, request.Password);
    }

    public async Task LogOut(string username, string deviceId)
    {
        var user = await _userManager.FindByNameAsync(username);
        var device = user.AuthenticatedDevices.Find(d => d.DeviceId.ToString() == deviceId);
        device.RefreshTokenExpiryTime = DateTime.Now;

        await _userManager.UpdateAsync(user);
    }
}