using AuthApi.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Entities;

public class User : IdentityUser
{
    public virtual List<AuthenticatedDevice> AuthenticatedDevices { get; set; } = new();
}