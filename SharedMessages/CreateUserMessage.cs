namespace SharedModels;

public record CreateUserMessage
{
    public string Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}