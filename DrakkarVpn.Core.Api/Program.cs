using DrakkarVpn.Core.Api.Extensions;
using DrakkarVpn.Core.Api.Middlewares;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Infrastructure.BackgroundWorkers;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure;
using DrakkarVpn.Core.Api.Modules.Servers.Application;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure;
using DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure;
using DrakkarVpn.Core.Api.Modules.Users.Application;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddApiBasics();
services.AddCoreApplication(
    typeof(UsersApplicationMarker).Assembly,
    typeof(ServerApplicationMarker).Assembly);

services.AddCoreInfrastructure(configuration.GetConnectionString("Default"));
services.AddUsersInfrastructure();
services.AddServersInfrastructure();
services.AddPeersInfrastructure();
services.AddAgentInfrastructure();
services.AddTariffsInfrastructure();
services.AddHostedService<ServerHealthBackgroundWorker>();
services.AddHttpClient();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandling();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();