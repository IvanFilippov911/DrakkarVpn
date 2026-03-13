using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.DeleteCoreAlert;

public sealed record DeleteCoreAlertCommand(Guid Id) : IRequest<bool>;