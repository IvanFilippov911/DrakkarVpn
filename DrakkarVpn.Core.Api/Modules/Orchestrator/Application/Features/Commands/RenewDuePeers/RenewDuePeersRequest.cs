using MediatR;
namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.RenewDuePeers;

public sealed record RenewDuePeersRequest() : IRequest<int>;