using MinioHosting.Extensions;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);


var sqlPassword = builder.AddParameter("sql-password", true);

var postgres = builder.AddPostgres("postgres", password: sqlPassword, port: 9999)
    .WithBindMount("./data/postgres", "/var/lib/postgresql/data");
//var postgres = builder.AddPostgres("postgres", password: sqlPassword, port: 9999);

var postgresdb = postgres.AddDatabase("postgresdb");

var redis = builder.AddRedis("rediscache");

var messaging = builder.AddRabbitMQ("rabbitmq").WithDockerfile("RaabbitMQSetting");

var minioPassword = builder.AddParameter("minio-password", true);

var minio = builder.AddMinioContainer("minio", minioPort: 9000, rootPassword: minioPassword)
    .WithBindMount("./data/minio", "/data");


var migrations = builder.AddProject<NotTinder_MigrationService>("migrationservice").WithReference(postgresdb)
    .WaitFor(postgresdb);

var seed = builder.AddProject<NotTinderApi_SeedService>("seedservice").WithReference(postgresdb)
    .WaitForCompletion(migrations);

builder.AddProject<AuthApi>("authapi").WithReference(postgresdb).WithReference(messaging)
    .WaitForCompletion(migrations);

builder.AddProject<GateWayApi>("gatewayapi");

builder.AddProject<CoinRateApi>("coinrateapi").WithReference(postgresdb).WaitForCompletion(migrations);

builder.AddProject<UsersApi>("usersapi").WithReference(postgresdb).WithReference(messaging).WithReference(redis)
    .WaitForCompletion(migrations);
builder.AddProject<StartupsApi>("startupsapi").WithReference(minio).WithReference(postgresdb)
    .WithReference(redis).WithReference(messaging).WaitForCompletion(migrations);

builder.AddProject<TransactionsApi>("transactionapi").WithReference(postgresdb).WithReference(messaging)
    .WaitForCompletion(migrations);


builder.Build().Run();