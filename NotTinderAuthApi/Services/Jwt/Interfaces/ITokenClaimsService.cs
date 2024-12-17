using Entities;

namespace TokenServices;

public interface ITokenClaimsService
{
    public Task<User> GetUserFromAccessToken(string token);
    public string GetDeviceIdFromAccessToken(string token);
}