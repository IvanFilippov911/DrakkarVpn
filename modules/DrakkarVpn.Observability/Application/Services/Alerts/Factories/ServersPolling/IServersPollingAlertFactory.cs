using DrakkarVpn.Observability.Application.Commands;
using DrakkarVpn.Observability.Application.DTOs;

namespace DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.ServersPolling;

public interface IServersPollingAlertFactory
{
    IReadOnlyList<CreateCoreAlertArgs> Build(ServersPollingCycleDto cycle);
}