using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using TransactionsApi.Consumers;
using TransactionsApi.Context;
using TransactionsApi.Saga;

var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDbContext<TransactionSagaDbContext>("postgresdb");
builder.AddNpgsqlDbContext<TransactionDbContext>("postgresdb");


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<TransactionCompletedConsumer>();

    configurator.SetKebabCaseEndpointNameFormatter();
    configurator.AddDelayedMessageScheduler();
    configurator.AddSagaStateMachine<TransactionStateMachine, TransactionSagaState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
            r.ExistingDbContext<TransactionSagaDbContext>();
            r.LockStatementProvider = new PostgresLockStatementProvider();
        });


    configurator.UsingRabbitMq((context, config) =>
    {
        config.UseInMemoryOutbox(context);
        config.UseDelayedMessageScheduler();

        var configService = context.GetRequiredService<IConfiguration>();
        var connectionString = configService.GetConnectionString("rabbitmq");
        config.Host(connectionString);

        config.ConfigureEndpoints(context);
    });
});


var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();


app.Run();