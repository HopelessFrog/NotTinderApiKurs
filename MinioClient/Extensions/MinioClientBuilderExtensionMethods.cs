using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minio;
using MinioClient.Components;

namespace MinioClient.Extensions;

public static class MinioClientBuilderExtensionMethods
{
    private const string DefaultConfigSectionName = "Aspire:Minio:Client";

    public static void AddMinio(this IHostApplicationBuilder builder, string connectionName)
    {
        ArgumentNullException.ThrowIfNull(builder);

        MinioConfiguration minioSettings = new();
        builder.Configuration.GetSection($"{DefaultConfigSectionName}:{connectionName}").Bind(minioSettings);

        if (builder.Configuration.GetConnectionString(connectionName) is string connectionString)
            minioSettings.ParseConnectionString(connectionString);


        var endpoint = minioSettings.Endpoint;
        var port = minioSettings.Port;
        var accessKey = minioSettings.AccessKey;
        var secretKey = minioSettings.SecretKey;

        builder.Services.AddMinio(configureClient => configureClient
                .WithEndpoint(endpoint, port)
                .WithSSL(false)
                .WithCredentials(accessKey, secretKey))
            .BuildServiceProvider();

        //TODO add healthcheck and metricks
    }
}