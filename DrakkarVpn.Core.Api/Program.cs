using DrakkarVpn.Core.Api.Extensions;
using DrakkarVpn.Core.Api.Middlewares;
using DrakkarVpn.Core.Api.Modules.Users.Application;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddApiBasics();
services.AddUsersApplication();
services.AddUsersInfrastructure(configuration.GetConnectionString("Default")); 

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