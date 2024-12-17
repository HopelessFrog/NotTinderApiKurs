namespace GateWayApi.CustomAggregators.Models;

public class DonaterDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = "";
    public int Donated { get; set; }
}