namespace StartupsApi.Models.Entity;

public class Donater
{
    public int Id { get; set; }
    public Guid UserId { get; set; }

    public int Donated { get; set; }
}