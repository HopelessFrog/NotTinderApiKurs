namespace SharedModels.TransactionMessages;

public record DebitUserRequest
{
    public Guid TransactionId { get; set; }
    public Guid UserId { get; set; }
    public int Amount { get; set; }
}