using Microsoft.EntityFrameworkCore;
using StartupsApi.Models.Entity;

namespace StartupsApi.Context;

public class StartupsDbContext : DbContext
{
    public StartupsDbContext(DbContextOptions options) : base(options)
    {
    }


    public DbSet<Startup> Startups { get; set; }
    public DbSet<Donater> Donaters { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Db");
    }
}