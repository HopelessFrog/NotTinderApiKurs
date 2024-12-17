using Microsoft.EntityFrameworkCore;
using UsersApi.Models.Entitys;

namespace UsersApi.Context;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Db");
    }
}