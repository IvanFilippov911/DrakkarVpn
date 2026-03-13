using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Observability.Application.Commands;

namespace DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.PeerTraffic;

public interface IPeerTrafficAlertFactory
{
    IReadOnlyList<CreateCoreAlertArgs> Build(SuspiciousPeerTrafficDto s);
}