using CoinRateApi.Context;
using CoinRateApi.Models;

namespace CoinRateApi.Services;

public class RandomRateGeneratorService : BackgroundService
{
    private readonly Random _random = new();
    private readonly IServiceProvider _serviceProvider;

    public RandomRateGeneratorService(IServiceProvider provider)
    {
        _serviceProvider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await GenerateRandomValue();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task GenerateRandomValue()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CoinRateContext>();
            var coinRate = new CoinRate
            {
                Value = _random.Next(10, 100),
                DateTime = DateTime.Now
            };

            context.CoinRates.Add(coinRate);
            await context.SaveChangesAsync();
        }
    }
}