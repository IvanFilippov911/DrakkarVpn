using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Errors.DeleteCoreErrorEvent;

public sealed record DeleteCoreErrorEventCommand(Guid Id) : IRequest<bool>;