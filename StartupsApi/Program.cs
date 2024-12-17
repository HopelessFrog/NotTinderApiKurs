using MassTransit;
using Microsoft.EntityFrameworkCore;
using MinioClient.Extensions;
using Redis.Cache;
using StartupsApi;
using StartupsApi.Consumers;
using StartupsApi.Context;
using StartupsApi.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddNpgsqlDbContext<StartupsDbContext>("postgresdb",
    configureDbContextOptions: options => options.UseLazyLoadingProxies());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddMinio("minio");
builder.AddRedisClient("rediscache");
builder.Services.AddControllers();

builder.Services.AddScoped<IMinioService, MinioService>();
builder.Services.AddScoped<IStartupsService, StartupsService>();

builder.Services.AddRedisCacheService();

builder.Services.AddMassTransit(configurator =>
{
    configurator.SetKebabCaseEndpointNameFormatter();

    configurator.AddDelayedMessageScheduler();
    configurator.AddConsumer<CreditStartupConsumer>();


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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();