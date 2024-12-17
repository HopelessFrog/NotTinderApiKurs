using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using TransactionsApi.Maps;

namespace TransactionsApi.Context;

public class TransactionSagaDbContext : SagaDbContext
{
    public TransactionSagaDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations => new ISagaClassMap[]
    {
        new TransactionSagaStateMap()
    };
}