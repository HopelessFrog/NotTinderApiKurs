﻿using Ocelot.Authorization;

namespace GateWayApi.ClaimsFix;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection DecorateClaimAuthoriser(this IServiceCollection services)
    {
        var serviceDescriptor = services.First(x => x.ServiceType == typeof(IClaimsAuthorizer));
        services.Remove(serviceDescriptor);

        var newServiceDescriptor = new ServiceDescriptor(serviceDescriptor.ImplementationType,
            serviceDescriptor.ImplementationType, serviceDescriptor.Lifetime);
        services.Add(newServiceDescriptor);

        services.AddTransient<IClaimsAuthorizer, ClaimAuthorizerDecorator>();

        return services;
    }
}