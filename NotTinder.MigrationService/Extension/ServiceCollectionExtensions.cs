using Microsoft.EntityFrameworkCore;

namespace NotTinder.MigrationService.Extension;

public static class ServiceCollectionExtensions
{
    public static IEnumerable<Type> GetAllDbContexts(this IServiceCollection services)
    {
        return services
            .Where(sd => sd.ServiceType.IsAssignableTo(typeof(DbContext)))
            .Select(sd => sd.ServiceType);
    }
}