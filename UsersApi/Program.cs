using AuthApi.Services;
using MassTransit;
using Redis.Cache;
using UsersApi.Consumers;
using UsersApi.Context;
using UsersApi.Services;
using UsersApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddNpgsqlDbContext<UserContext>("postgresdb");

builder.AddRedisClient("rediscache");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IRegisterService, RegisterService>();
builder.Services.AddScoped<IUserBalanceService, UserBalanceService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddRedisCacheService();


builder.Services.AddMassTransit(configurator =>
{
    configurator.SetKebabCaseEndpointNameFormatter();
    configurator.AddDelayedMessageScheduler();

    configurator.AddConsumer<DebitUserConsumer>();
    configurator.AddConsumer<RefundUserConsumer>();

    configurator.UsingRabbitMq((context, config) =>
    {
        config.UseInMemoryOutbox(context);
        config.UseDelayedMessageScheduler();

        var configService = context.GetRequiredService<IConfiguration>();
        var connectionString = configService.GetConnectionString("rabbitmq");
        config.Host(connectionString);

        config.ReceiveEndpoint("usersrefunds", e => { e.ConfigureConsumer<RefundUserConsumer>(context); });

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