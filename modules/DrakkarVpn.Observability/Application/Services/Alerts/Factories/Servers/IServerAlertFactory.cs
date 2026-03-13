using DrakkarVpn.Observability.Application.Commands;
using DrakkarVpn.Observability.Application.DTOs;

namespace DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.Servers;

public interface IServerAlertFactory
{
    IReadOnlyList<CreateCoreAlertArgs> Build(
        ServerInfoUpdateDto u,
        DateTime nowUtc);
}