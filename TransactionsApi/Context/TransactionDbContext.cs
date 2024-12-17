using Microsoft.EntityFrameworkCore;
using TransactionsApi.Models.Entitys;

namespace TransactionsApi.Context;

public class TransactionDbContext : DbContext
{
    public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
    {
    }

    public TransactionDbContext()
    {
    }

    public DbSet<Transaction> TransactionsStats { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Db");
    }
}