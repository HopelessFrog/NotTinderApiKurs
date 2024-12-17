using MassTransit;
using SharedModels.TransactionMessages;
using UsersApi.Services.Interfaces;

namespace UsersApi.Consumers;

public class DebitUserConsumer : IConsumer<DebitUserRequest>
{
    private readonly IUserBalanceService _userBalanceService;

    public DebitUserConsumer(IUserBalanceService userBalanceService)
    {
        _userBalanceService = userBalanceService;
    }

    public async Task Consume(ConsumeContext<DebitUserRequest> context)
    {
        await _userBalanceService.TopDownBalance(context.Message.Amount, context.Message.UserId);

        await context.RespondAsync(new DebitUserResponse { TransactionId = context.Message.TransactionId });
    }
}