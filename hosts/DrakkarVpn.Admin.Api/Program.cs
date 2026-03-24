using DrakkarVpn.Admin.Api.DI;
using DrakkarVpn.AdminAuth.DI;
using DrakkarVpn.HostInfrastructure;
using DrakkarVpn.HostInfrastructure.Infrastructure.Middlewares;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.AddHostLogging("Drakkar.Core.Admin");

var services = builder.Services;
var configuration = builder.Configuration;

services.AddAdminHostApi()
    .AddFrontendCors(configuration, builder.Environment)
    .AddForwardedHeadersSupport(configuration, builder.Environment);

services.AddAdminHostComposition(configuration);
services.AddDrakkarMediatR();
services.AddAdminInfrastructureModule(configuration);
services.AddAdminHostUnitOfWorkBehaviors();

var app = builder.Build();

await app.Services.SeedAdminAuthAsync();

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