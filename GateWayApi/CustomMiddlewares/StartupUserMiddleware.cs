using System.Net;
using System.Text;
using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;

namespace GateWayApi.CustomMiddlewares;

public class StartupUserMiddleware : IDefinedAggregator
{
    public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
    {
      
        
        
    }
}