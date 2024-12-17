using System.IdentityModel.Tokens.Jwt;
using AuthApi.Context;
using CoinRateApi.Context;
using NotTinder.MigrationService;
using StartupsApi.Context;
using TransactionsApi.Context;
using UsersApi.Context;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("Migrations"));

builder.AddNpgsqlDbContext<CoinRateContext>("postgresdb");
builder.AddNpgsqlDbContext<AuthDbContext>("postgresdb");
builder.AddNpgsqlDbContext<UserContext>("postgresdb");
builder.AddNpgsqlDbContext<StartupsDbContext>("postgresdb");
builder.AddNpgsqlDbContext<TransactionSagaDbContext>("postgresdb");
builder.AddNpgsqlDbContext<TransactionDbContext>("postgresdb");


builder.Services.AddSingleton(builder.Services);

var host = builder.Build();
host.Run();