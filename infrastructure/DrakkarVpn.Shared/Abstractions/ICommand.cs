using MediatR;

namespace DrakkarVpn.Shared.Abstractions;

public interface ICommand<out TResponse> : IRequest<TResponse>;
public interface ICommand : IRequest;

