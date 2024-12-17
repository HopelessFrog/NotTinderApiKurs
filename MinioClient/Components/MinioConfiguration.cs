using System.Data.Common;

namespace MinioClient.Components;

public class MinioConfiguration
{
    private const string HostString = "host";
    private const string PortString = "port";
    private const string AccesKeyString = "accessKey";
    private const string SecretKeyString = "secretKey";

    public string? Endpoint { get; set; }

    public int Port { get; set; }
    public string? AccessKey { get; set; }

    public string? SecretKey { get; set; }

    public bool HealthChecks { get; set; } = true;

    public bool Tracing { get; set; } = true;

    public bool Metrics { get; set; } = true;

    internal void ParseConnectionString(string? connectionString)
    {
        var connectionBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString
        };

        if (connectionBuilder.TryGetValue(HostString, out var endpoint)) Endpoint = endpoint.ToString();
        if (connectionBuilder.TryGetValue(PortString, out var port)) Port = Convert.ToInt32(port);
        if (connectionBuilder.TryGetValue(AccesKeyString, out var accessKey)) AccessKey = accessKey.ToString();
        if (connectionBuilder.TryGetValue(SecretKeyString, out var secretKey)) SecretKey = secretKey.ToString();
    }
}