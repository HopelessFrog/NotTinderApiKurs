using MassTransit;
using SharedModels.TransactionMessages;
using StartupsApi.Interfaces;

namespace StartupsApi.Consumers;

public class CreditStartupConsumer : IConsumer<CreditStartupRequest>
{
    private readonly IStartupsService _startupsService;

    public CreditStartupConsumer(IStartupsService startupsService)
    {
        _startupsService = startupsService;
    }

    public async Task Consume(ConsumeContext<CreditStartupRequest> context)
    {
        await _startupsService.CreditStartup(context.Message.StartupId, context.Message.UserId, context.Message.Amount);

        await context.RespondAsync(new CreditStartupResponse { TransactionId = context.Message.TransactionId });
    }
}