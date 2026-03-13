using DrakkarVpn.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Handlers.RunServersPolling;

public sealed record RunServersPollingCommand(
    int BatchSize,
    TimeSpan LeaseDuration,
    TimeSpan StuckTimeout,
    int HttpConcurrency
) : IRequest<Unit>, IServersCommand<Unit>;