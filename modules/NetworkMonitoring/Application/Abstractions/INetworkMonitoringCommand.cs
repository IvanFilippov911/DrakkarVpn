using DrakkarVpn.Shared.Abstractions;

namespace NetworkMonitoring.Application.Abstractions;

public interface INetworkMonitoringCommand<out TResponse> : ICommand<TResponse>;

