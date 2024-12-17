namespace AuthApi.Models.Requests;

public class AuthRequest
{
    public string? DeviceId { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public bool Remember { get; set; }
}