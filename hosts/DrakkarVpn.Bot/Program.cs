using System.Reflection;
using DrakkarVpn.Bot.Infrastructure.Hosting;
using DrakkarVpn.Bot.Infrastructure.Telegram.Options;
using DrakkarVpn.Bot.Infrastructure.Telegram.UpdateHandling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<TelegramOptions>(
            context.Configuration.GetSection(TelegramOptions.SectionName));

        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<TelegramOptions>>().Value;
            return new TelegramBotClient(opts.Token);
        });

        services.AddHostedService<BotWorker>();
        services.AddSingleton<UpdateHandler>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    })
    .Build();

await host.RunAsync();