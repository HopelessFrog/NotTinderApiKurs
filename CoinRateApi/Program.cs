using CoinRateApi.Context;
using CoinRateApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddNpgsqlDbContext<CoinRateContext>("postgresdb");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddJwtConfiguration(builder.Configuration);
//builder.Services.AddCustomJwtAuthentication(builder.Configuration.GetSection("Jwt").Get<JwtConfiguration>());

builder.Services.AddHostedService<RandomRateGeneratorService>();

builder.Services.AddControllers();

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

app.UseAuthorization();
app.UseAuthentication();


app.MapControllers();

app.Run();