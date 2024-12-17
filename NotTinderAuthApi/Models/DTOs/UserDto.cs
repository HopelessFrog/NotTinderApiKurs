namespace AuthApi.Models.DTOs;

public class UserDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<string> Roles { get; set; }
}