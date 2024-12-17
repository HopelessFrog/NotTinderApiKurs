using Microsoft.EntityFrameworkCore;

namespace NotTinder.MigrationService.Extension;

public static class ServiceProviderExtensions
{
    public static IEnumerable<DbContext> GetAllDbContextInstances(this IServiceProvider serviceProvider)
    {
        var dbContextTypes = serviceProvider.GetService<IServiceCollection>().GetAllDbContexts();
        using (var scope = serviceProvider.CreateScope())
        {
            foreach (var dbContextType in dbContextTypes)
                yield return scope.ServiceProvider.GetService(dbContextType) as DbContext;
        }
    }
}