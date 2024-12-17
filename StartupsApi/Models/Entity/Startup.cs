namespace StartupsApi.Models.Entity;

public class Startup
{
    public int Id { get; set; }

    public Guid OwnerId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int DonationTarget { get; set; }


    public virtual List<Donater> Donaters { get; set; } = new();
}