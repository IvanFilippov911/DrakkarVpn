using System.Reflection;
using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.Metrics.Queries.AgentMetrics;
using DrakkarVpn.Agent.Application.Peers.Queries.PeerMetrics;
using DrakkarVpn.Agent.Application.Services;
using DrakkarVpn.Agent.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddMemoryCache();

services.AddXrayGrpc(builder.Configuration);
services.AddSingleton<INetworkMetricsService, NetworkMetricsService>();
services.AddScoped<IBenchmarkService, BenchmarkService>();
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetAgentMetricsHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetPeerMetricsHandler).Assembly);
});

services.AddHttpClient("grpc")
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
        KeepAlivePingDelay = TimeSpan.FromSeconds(30),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(10),
    });

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();


app.MapControllers();

app.Run();