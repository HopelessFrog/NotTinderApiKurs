namespace SharedModels.TransactionMessages;

public record DebitUserResponse
{
    public Guid TransactionId { get; set; }
}