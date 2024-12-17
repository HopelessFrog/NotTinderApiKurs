namespace MinioHosting.Components;

public class MinioContainerResource : ContainerResource, IResourceWithConnectionString
{
    internal const string PrimaryEndpointName = "tcp";
    internal const string SecondaryEndpointName = "http";

    private const string DefaultUserName = "minio";

    private EndpointReference? _primaryEndpoint;

    public MinioContainerResource(string name, ParameterResource? userName, ParameterResource password) : base(name)
    {
        PrimaryEndpoint = new EndpointReference(this, PrimaryEndpointName);
        UserNameParameter = userName;
        ArgumentNullException.ThrowIfNull(password);
        PasswordParameter = password;
    }

    public EndpointReference PrimaryEndpoint { get; }

    public ParameterResource? UserNameParameter { get; }

    internal ReferenceExpression UserNameReference =>
        UserNameParameter is not null
            ? ReferenceExpression.Create($"{UserNameParameter}")
            : ReferenceExpression.Create($"{DefaultUserName}");

    public ParameterResource PasswordParameter { get; }


    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create(
            $"host={PrimaryEndpoint.Property(EndpointProperty.Host)};port={PrimaryEndpoint.Property(EndpointProperty.Port)};accessKey={UserNameReference};secretKey={PasswordParameter};");
}