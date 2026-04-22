

using DrakkarVpn.AdminAuth;
using DrakkarVpn.AdminAuth.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure;
using DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure.EF;
using DrakkarVpn.Idempotency;
using DrakkarVpn.Observability;
using DrakkarVpn.Observability.Infrastructure.EF;
using DrakkarVpn.Peers;
using DrakkarVpn.Servers;
using DrakkarVpn.Subscriptions;
using DrakkarVpn.Tariffs;
using DrakkarVpn.Users;
using DrakkarVpn.Users.Infrastructure.EF;
using Idempotency.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetworkMonitoring;
using NetworkMonitoring.Infrastructure.EF;

var builder = Host.CreateApplicationBuilder(args);

var services = builder.Services;
var config = builder.Configuration;


services.AddUsersInfrastructure(config);
services.AddAdminAuthInfrastructure(config);
services.AddPeersInfrastructure(config);
services.AddServersInfrastructure(config);
services.AddSubscriptionsInfrastructure(config);
services.AddTariffsInfrastructure(config);
services.AddObservabilityInfrastructure(config);
services.AddIdempotencyInfrastructure(config);
services.AddNetworkMonitoringInfrastructure(config);

var app = builder.Build();

using var scope = app.Services.CreateScope();
var sp = scope.ServiceProvider;

await sp.GetRequiredService<UsersDbContext>().Database.MigrateAsync();
await sp.GetRequiredService<AdminAuthDbContext>().Database.MigrateAsync();
await sp.GetRequiredService<PeerDbContext>().Database.MigrateAsync();
await sp.GetRequiredService<ServerDbContext>().Database.MigrateAsync();
await sp.GetRequiredService<SubscriptionDbContext>().Database.MigrateAsync();
await sp.GetRequiredService<TariffsDbContext>().Database.MigrateAsync();
await sp.GetRequiredService<ObservabilityDbContext>().Database.MigrateAsync();
await sp.GetRequiredService<IdempotencyDbContext>().Database.MigrateAsync();
await sp.GetRequiredService<NetworkMonitoringDbContext>().Database.MigrateAsync();

Console.WriteLine("✅ All migrations applied");