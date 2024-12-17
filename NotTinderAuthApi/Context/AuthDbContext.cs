using AuthApi.Models.Entities;
using Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Context;

public class AuthDbContext : IdentityDbContext<User>
{
    public AuthDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AuthenticatedDevice> AuthenticatedDevices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Db");
    }
}