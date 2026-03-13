using DrakkarVpn.Shared.Abstractions;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;

public interface IUsersCommand<out TResponse> : ICommand<TResponse>;
public interface IUsersCommand : ICommand;