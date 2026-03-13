using DrakkarVpn.Admin.Api.DI;
using DrakkarVpn.HostInfrastructure;
using DrakkarVpn.HostInfrastructure.Infrastructure.Middlewares;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.AddHostLogging("Drakkar.Core.Admin");

var services = builder.Services;
var configuration = builder.Configuration;

services.AddAdminHostApi()
    .AddLocalCorsForFrontend();

services.AddAdminHostComposition(configuration);
services.AddDrakkarMediatR();
services.AddAdminInfrastructureModule(configuration);
services.AddUnitOfWorkBehaviors();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandling();

app.UseHttpMetrics();
app.MapControllers();
app.MapMetrics();

app.Run();