using MassTransit;
using SharedModels.TransactionMessages;
using UsersApi.Services.Interfaces;

namespace UsersApi.Consumers;

public class RefundUserConsumer : IConsumer<RefundUser>
{
    private readonly IUserBalanceService _userBalanceService;

    public RefundUserConsumer(IUserBalanceService userBalanceService)
    {
        _userBalanceService = userBalanceService;
    }

    public async Task Consume(ConsumeContext<RefundUser> context)
    {
        await _userBalanceService.TopUpBalance(context.Message.Amount, context.Message.UserId);
    }
}