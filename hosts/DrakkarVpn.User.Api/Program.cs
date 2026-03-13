using DrakkarVpn.Core.Api.DI;
using DrakkarVpn.Core.Api.Extensions;
using DrakkarVpn.HostInfrastructure;
using DrakkarVpn.HostInfrastructure.Infrastructure.Middlewares;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.AddHostLogging("Drakkar.Core.User");

var services = builder.Services;
var configuration = builder.Configuration;

services.AddUserHostApi()
    .AddSwaggerJwt()
    .AddLocalCorsForFrontend();

services.AddDrakkarMediatR();

services.AddUserHostComposition(configuration);
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