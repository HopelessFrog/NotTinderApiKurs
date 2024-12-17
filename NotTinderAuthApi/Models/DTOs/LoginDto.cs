namespace AuthApi.Models.DTOs;

public class LoginDto
{
    public string Id { get; set; }
    public string Name { get; set; }

    public TokenDto Tokens { get; set; }

    public List<string> Roles { get; set; } = new();
}