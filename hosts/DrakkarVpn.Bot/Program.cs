using System.Reflection;
using DrakkarVpn.Bot.Application.Abstractions;
using DrakkarVpn.Bot.Infrastructure.Hosting;
using DrakkarVpn.Bot.Infrastructure.Integrations.UserFlowApi;
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

        services.AddHttpClient<IUserFlowClient, UserFlowClient>((sp, client) =>
        {
            var baseUrl = context.Configuration["UserFlowApi:BaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Configuration UserFlowApi:BaseUrl is missing");

            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });
    })
    .Build();

await host.RunAsync();