using DrakkarVpn.Core.Api.Extensions;
using DrakkarVpn.Core.Api.Middlewares;
using DrakkarVpn.Core.Api.Modules.Users.Application;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddApiBasics();
services.AddUsersApplication();

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