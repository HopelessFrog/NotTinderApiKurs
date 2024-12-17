using GateWayApi.ClaimsFix;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using TokenShared;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", false, true)
    .AddEnvironmentVariables();


builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.DecorateClaimAuthoriser();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddCustomJwtAuthentication();

builder.Services.AddHttpClient();

builder.Services.AddMemoryCache();

builder.Services.AddControllers();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.UseOcelot().Wait();


app.Run();