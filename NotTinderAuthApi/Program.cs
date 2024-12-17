using AuthApi.Consumers;
using AuthApi.Context;
using AuthApi.Services;
using AuthApi.Services.Interfaces;
using Entities;
using JwtAuthentication;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TokenServices;
using TokenShared;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.AddNpgsqlDbContext<AuthDbContext>("postgresdb",
    configureDbContextOptions: options => options.UseLazyLoadingProxies());


builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 2;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireNonAlphanumeric = false;
    }).AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateUserConsumer>();

    x.UsingRabbitMq((context, config) =>
    {
        config.ReceiveEndpoint(e => { e.ConfigureConsumer<CreateUserConsumer>(context); });

        var configService = context.GetRequiredService<IConfiguration>();
        var connectionString = configService.GetConnectionString("rabbitmq");
        config.Host(connectionString);
    });
});


builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddTransient<ITokenClaimsService, TokenClaimsService>();

builder.Services.AddCustomJwtAuthentication();


builder.Services.AddTransient<ITokenGenerator, TokenGenerator>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.MapDefaultEndpoints();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();