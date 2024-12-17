using MassTransit;
using MassTransit.Contracts;
using SharedModels.TransactionMessages;
using TransactionsApi.Messages;

namespace TransactionsApi.Saga;

public sealed class TransactionStateMachine : MassTransitStateMachine<TransactionSagaState>
{
    public TransactionStateMachine()
    {
        BuildStateMachine();
        Initially(WhenTransactionRecived());

        During(DebitUser.Pending, WhenUserDebited(), WhenUserDebitDeclined(), WhenUserDebitTimeoutExpired());

        During(CreditStartup.Pending, WhenStartupCredited(), WhenStartupCreditDeclined(),
            WhenStartupCreditTimeoutExpired());


        SetCompletedWhenFinalized();
    }

    public Event<DonateToStartupRequest> DonateToStartup { get; set; }

    public State Failed { get; set; }

    public Request<TransactionSagaState, DebitUserRequest, DebitUserResponse> DebitUser { get; set; }

    public Request<TransactionSagaState, CreditStartupRequest, CreditStartupResponse> CreditStartup { get; set; }

    private void BuildStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => DonateToStartup, x => x.CorrelateById(m => m.Message.TransactionId));
        Request(
            () => DebitUser
        );
        Request(
            () => CreditStartup
        );
    }

    private EventActivityBinder<TransactionSagaState, DonateToStartupRequest> WhenTransactionRecived()
    {
        return When(DonateToStartup)
            .Then(context =>
            {
                context.Saga.UserId = context.Message.UserId;
                context.Saga.StartupId = context.Message.StartupId;
                context.Saga.Amount = context.Message.Amount;
            })
            .Then(x =>
            {
                if (!x.TryGetPayload(out SagaConsumeContext<TransactionSagaState, DonateToStartupRequest> payload))
                    throw new Exception("Unable to retrieve required payload for callback data.");
                x.Saga.RequestId = payload.RequestId;
                x.Saga.ResponseAddress = payload.ResponseAddress;
            })
            .Request(DebitUser,
                context => context.Init<DebitUserRequest>(
                    new DebitUserRequest
                    {
                        TransactionId = context.Saga.CorrelationId, UserId = context.Saga.UserId,
                        Amount = context.Saga.Amount
                    })
            )
            .TransitionTo(DebitUser.Pending);
    }

    private EventActivityBinder<TransactionSagaState, DebitUserResponse> WhenUserDebited()
    {
        return When(DebitUser.Completed)
            .Request(CreditStartup,
                context => context.Init<CreditStartupRequest>(new CreditStartupRequest
                {
                    TransactionId = context.Saga.CorrelationId,
                    StartupId = context.Saga.StartupId,
                    UserId = context.Saga.UserId,
                    Amount = context.Saga.Amount
                }))
            .TransitionTo(CreditStartup.Pending);
    }

    private EventActivityBinder<TransactionSagaState, RequestTimeoutExpired<DebitUserRequest>>
        WhenUserDebitTimeoutExpired()
    {
        return When(DebitUser.TimeoutExpired)
            .ThenAsync(async context =>
            {
                await RespondFromSaga(context, "Server is not responding, try again later.");
            })
            .TransitionTo(Failed);
    }


    private EventActivityBinder<TransactionSagaState, Fault<DebitUserRequest>> WhenUserDebitDeclined()
    {
        return When(DebitUser.Faulted)
            .ThenAsync(async context =>
            {
                await RespondFromSaga(context, context.Data.Exceptions?.FirstOrDefault()?.Message
                                               ?? "An unknown error occurred");
            })
            .TransitionTo(Failed);
    }

    private EventActivityBinder<TransactionSagaState, CreditStartupResponse> WhenStartupCredited()
    {
        return When(CreditStartup.Completed)
            .ThenAsync(async context => { await RespondFromSaga(context, null); })
            .Publish(context => new TransactionCompleted
            {
                TransactionId = context.Saga.CorrelationId,
                UserId = context.Saga.UserId,
                StartupId = context.Saga.StartupId,
                Amount = context.Saga.Amount
            })
            .Finalize();
    }

    private EventActivityBinder<TransactionSagaState, RequestTimeoutExpired<CreditStartupRequest>>
        WhenStartupCreditTimeoutExpired()
    {
        return When(CreditStartup.TimeoutExpired)
            .Send(new Uri("queue:usersrefunds"),
                context => new RefundUser
                {
                    TransactionId = context.Saga.CorrelationId, UserId = context.Saga.UserId,
                    Amount = context.Saga.Amount
                })
            .ThenAsync(async context =>
            {
                await RespondFromSaga(context, "Server is not responding, try again later.");
            })
            .TransitionTo(Failed);
    }


    private EventActivityBinder<TransactionSagaState, Fault<CreditStartupRequest>> WhenStartupCreditDeclined()
    {
        return When(CreditStartup.Faulted)
            .Send(new Uri("queue:usersrefunds"),
                context => new RefundUser
                {
                    TransactionId = context.Saga.CorrelationId,
                    UserId = context.Saga.UserId,
                    Amount = context.Saga.Amount
                })
            .ThenAsync(async context =>
            {
                await RespondFromSaga(context, context.Data.Exceptions?.FirstOrDefault()?.Message
                                               ?? "An unknown error occurred");
            })
            .TransitionTo(Failed);
    }

    private static async Task RespondFromSaga<T>(BehaviorContext<TransactionSagaState, T> context, string result)
        where T : class
    {
        var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
        await endpoint.Send(new DonateToStartupResponse { TransactionId = context.Saga.CorrelationId, Result = result }
            , r => r.RequestId = context.Saga.RequestId);
    }
}