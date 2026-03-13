using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Observability.Application.Commands;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Services.Alerts.CoreHealth;

public interface ICoreHealthAlertFactory
{
    IReadOnlyList<CreateCoreAlertArgs> Build(CoreHealthDto metrics);
}