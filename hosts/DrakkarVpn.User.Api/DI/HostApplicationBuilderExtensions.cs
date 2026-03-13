using Serilog;

namespace DrakkarVpn.Core.Api.DI;

public static class HostApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddHostLogging(this WebApplicationBuilder builder, string serviceName)
    {
        builder.Configuration.AddJsonFile("appsettings.Serilog.json", optional: false, reloadOnChange: true);

        builder.Host.UseSerilog((ctx, lc) => lc
            .ReadFrom.Configuration(ctx.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Service", serviceName)
        );

        return builder;
    }
}