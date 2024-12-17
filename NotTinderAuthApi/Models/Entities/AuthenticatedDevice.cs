namespace AuthApi.Models.Entities;

public class AuthenticatedDevice
{
    public int Id { get; set; }
    public string DeviceId { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}