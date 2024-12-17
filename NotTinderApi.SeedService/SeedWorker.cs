using System.Diagnostics;
using AuthApi.Context;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NotTinderApi.SeedService;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime,
    AuthDbContext context) : BackgroundService
{
    public const string ActivitySourceName = "Seed";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = s_activitySource.StartActivity("Seed admin", ActivityKind.Client);

        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole)) await roleManager.CreateAsync(new IdentityRole(adminRole));

        const string adminUsername = "admin";
        const string adminPassword = "admin";
        var adminUser = await userManager.FindByNameAsync(adminUsername);
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminUsername,
                Email = "admin@example.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, adminPassword);
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }


        hostApplicationLifetime.StopApplication();
    }
}