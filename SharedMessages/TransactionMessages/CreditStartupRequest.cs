namespace SharedModels.TransactionMessages;

public class CreditStartupRequest
{
    public Guid TransactionId { get; set; }
    public int StartupId { get; set; }
    public int Amount { get; set; }

    public Guid UserId { get; set; }
}