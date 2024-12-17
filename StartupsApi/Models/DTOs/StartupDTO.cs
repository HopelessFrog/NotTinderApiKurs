namespace StartupsApi.Models.DTOs;

public class StartupDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Images { get; set; }
    public Guid OwnerId { get; set; }
    public int DonationTarget { get; set; }
    public string Icon { get; set; }
}