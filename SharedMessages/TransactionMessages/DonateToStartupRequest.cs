namespace SharedModels.TransactionMessages;

public class DonateToStartupRequest
{
    public Guid TransactionId { get; set; }
    public Guid UserId { get; set; }
    public int StartupId { get; set; }
    public int Amount { get; set; }
}