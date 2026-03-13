using DrakkarVpn.Shared.Abstractions;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeersCommand<TResponse> : ICommand<TResponse>;