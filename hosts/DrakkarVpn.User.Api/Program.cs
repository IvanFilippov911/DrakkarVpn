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
    .AddFrontendCors(configuration, builder.Environment)
    .AddForwardedHeadersSupport(configuration, builder.Environment);

services.AddDrakkarMediatR();

services.AddUserHostComposition(configuration);
services.AddUserHostUnitOfWorkBehaviors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandling();

app.UseHttpMetrics();
app.MapControllers();
app.MapMetrics();

app.Run();