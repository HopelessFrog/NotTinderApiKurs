using AuthApi.Models.DTOs;
using AuthApi.Models.Requests;

namespace AuthApi.Services.Interfaces;

public interface IAuthService
{
    Task<bool> Login(AuthRequest request);
    Task<UserDto> GetUser(string username);
    Task LogOut(string username, string deviceId);
}