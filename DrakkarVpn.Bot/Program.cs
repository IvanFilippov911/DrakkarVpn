using System.Reflection;
using DrakkarVpn.Bot.Application.Abstraction;
using DrakkarVpn.Bot.Application.Features.Common;
using DrakkarVpn.Bot.Application.Pipeline;
using DrakkarVpn.Bot.Infrastructure.Telegram;
using DrakkarVpn.Bot.Infrastructure.Telegram.Abstraction;
using DrakkarVpn.Bot.Infrastructure.Telegram.Options;
using MediatR;
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
        
        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(classes => classes.AssignableTo<ITelegramCommandFactory>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        
        services.AddHostedService<BotWorker>();
        services.AddSingleton<ICommandRegistry, CommandRegistry>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ErrorHandlingBehavior<,>));
        services.AddSingleton<UpdateHandler>();
        
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        
        services.Configure<OrchestratorOptions>(
            context.Configuration.GetSection(OrchestratorOptions.SectionName));

        services.AddHttpClient<IOrchestratorClient, OrchestratorClient>((sp, client) =>
        {
            var opts = sp.GetRequiredService<IOptions<OrchestratorOptions>>().Value;
            client.BaseAddress = new Uri(opts.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });
        
        
        services.AddScoped<IUpdateHandlerStrategy, MessageUpdateStrategy>();
        services.AddScoped<IUpdateHandlerStrategy, CallbackUpdateStrategy>();




    })
    .Build();

await host.RunAsync();