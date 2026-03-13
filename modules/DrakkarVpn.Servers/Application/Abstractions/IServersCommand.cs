using DrakkarVpn.Shared.Abstractions;

namespace DrakkarVpn.Servers.Application.Abstractions;


public interface IServersCommand<TResponse> : ICommand<TResponse>;