namespace SharedModels.TransactionMessages;

public class RefundUser
{
    public Guid TransactionId { get; set; }
    public Guid UserId { get; set; }
    public int Amount { get; set; }
}