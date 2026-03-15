using DrakkarVpn.Shared.Abstractions;

namespace DrakkarVpn.AdminAuth.Application.Abstractions;

public interface IAdminAuthCommand<out TResponse> : ICommand<TResponse>;
public interface IAdminAuthCommand : ICommand;
