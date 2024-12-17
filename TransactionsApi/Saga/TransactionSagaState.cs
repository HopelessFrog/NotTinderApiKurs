using MassTransit;

namespace TransactionsApi.Saga;

public class TransactionSagaState : SagaStateMachineInstance
{
    public string CurrentState { get; set; }
    public Guid UserId { get; set; }
    public int StartupId { get; set; }
    public int Amount { get; set; }
    public Guid? RequestId { get; set; }
    public Uri? ResponseAddress { get; set; }
    public Guid CorrelationId { get; set; }
}