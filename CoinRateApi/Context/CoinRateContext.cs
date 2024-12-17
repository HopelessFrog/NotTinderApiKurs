using CoinRateApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoinRateApi.Context;

public class CoinRateContext : DbContext
{
    public CoinRateContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<CoinRate> CoinRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Db");
    }
}