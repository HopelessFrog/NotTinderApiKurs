using MassTransit;
using TransactionsApi.Context;
using TransactionsApi.Messages;
using TransactionsApi.Models.Entitys;

namespace TransactionsApi.Consumers;

public class TransactionCompletedConsumer : IConsumer<TransactionCompleted>
{
    private readonly TransactionDbContext _dbContext;

    public TransactionCompletedConsumer(TransactionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<TransactionCompleted> context)
    {
        var message = context.Message;

        _dbContext.TransactionsStats.Add(new Transaction
        {
            Id = message.TransactionId,
            UserId = message.UserId,
            StartupId = message.StartupId,
            Amount = message.Amount
        });

        await _dbContext.SaveChangesAsync();
    }
}