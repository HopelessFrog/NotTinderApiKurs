using MinioHosting.Components;
using NotTinderApi.AppHost.MinioHosting;

namespace MinioHosting.Extensions;

public static class MinioBuilderExtensions
{
    private const string RootUserEnvVarName = "MINIO_ROOT_USER";
    private const string RootPasswordEnvVarName = "MINIO_ROOT_PASSWORD";


    public static IResourceBuilder<MinioContainerResource> AddMinioContainer(
        this IDistributedApplicationBuilder builder,
        string name,
        IResourceBuilder<ParameterResource>? rootUser = null,
        IResourceBuilder<ParameterResource>? rootPassword = null,
        int minioPort = 9000,
        int minioAdminPort = 9001)
    {
        var passwordParameter = rootPassword?.Resource ??
                                ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(builder,
                                    $"{name}-password");
        var minioContainer = new MinioContainerResource(name, rootUser?.Resource, passwordParameter);

        return builder
            .AddResource(minioContainer)
            .WithEndpoint(minioPort, 9000, name: MinioContainerResource.PrimaryEndpointName)
            .WithHttpEndpoint(minioAdminPort, 9001,
                MinioContainerResource.SecondaryEndpointName)
            .WithImage(MinioContainerImageTags.Image)
            .WithImageRegistry(MinioContainerImageTags.Registry)
            .WithEnvironment("MINIO_ADDRESS", ":9000")
            .WithEnvironment("MINIO_CONSOLE_ADDRESS", ":9001")
            .WithEnvironment("MINIO_PROMETHEUS_AUTH_TYPE", "public")
            .WithEnvironment(context =>
            {
                context.EnvironmentVariables.Add(RootUserEnvVarName, minioContainer.UserNameReference);
                context.EnvironmentVariables.Add(RootPasswordEnvVarName, minioContainer.PasswordParameter);
            })
            .WithArgs("server", "/data");
    }
}