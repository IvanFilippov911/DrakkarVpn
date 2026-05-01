using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.Exception;
using DrakkarVpn.Agent.Application.Feature.Commands.ApplyServerTransport;
using DrakkarVpn.Agent.Application.Metrics.Queries.AgentMetrics;
using DrakkarVpn.Agent.Application.Peers.Queries.PeerMetrics;
using DrakkarVpn.Agent.Application.Services;
using FluentValidation;
using DrakkarVpn.Agent.Infrastructure.EF;
using DrakkarVpn.Agent.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddMemoryCache();

services.AddXrayGrpc(builder.Configuration);
services.AddAgentStorage(builder.Configuration);
services.AddSingleton<INetworkMetricsService, NetworkMetricsService>();
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetAgentMetricsHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetPeerMetricsHandler).Assembly);
});
services.AddValidatorsFromAssemblyContaining<ApplyServerTransportCommandValidator>();

services.AddHttpClient("grpc")
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
        KeepAlivePingDelay = TimeSpan.FromSeconds(30),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(10),
    });

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AgentDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler(handlerApp =>
{
    handlerApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var ex = feature?.Error;

        if (ex is PeersAgentApplyFailedException aex)
        {
            context.Response.StatusCode = StatusCodes.Status502BadGateway;
            await context.Response.WriteAsJsonAsync(new
            {
                code = aex.Code,
                message = aex.Message
            });
            return;
        }
        
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new { code = "internal_error" });
    });
});
app.UseAuthorization();


app.MapControllers();

app.Run();