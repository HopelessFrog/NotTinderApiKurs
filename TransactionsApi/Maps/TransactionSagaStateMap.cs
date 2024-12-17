using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransactionsApi.Saga;

namespace TransactionsApi.Maps;

public class TransactionSagaStateMap : SagaClassMap<TransactionSagaState>
{
    protected override void Configure(EntityTypeBuilder<TransactionSagaState> entity, ModelBuilder model)
    {
        base.Configure(entity, model);
        entity.Property(x => x.CurrentState).HasMaxLength(255);
    }
}