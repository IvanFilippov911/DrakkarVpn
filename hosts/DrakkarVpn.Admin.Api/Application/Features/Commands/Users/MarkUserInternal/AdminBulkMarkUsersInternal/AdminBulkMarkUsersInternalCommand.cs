using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Users.Response;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminBulkMarkUsersInternal;

public sealed record AdminBulkMarkUsersInternalCommand(
    IReadOnlyCollection<Guid> UserIds,
    bool IsInternal
) : IRequest<BulkUsersOperationResponse>, IUsersCommand<BulkUsersOperationResponse>;