namespace UsersApi.Models.Entitys;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int AvatarId { get; set; }
    public string Email { get; set; }
    public int Balance { get; set; }
}