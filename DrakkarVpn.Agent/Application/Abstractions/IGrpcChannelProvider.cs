using Grpc.Net.Client;

namespace DrakkarVpn.Agent.Infrastructure.Services.Grpc;

public interface IGrpcChannelProvider
{
    GrpcChannel GetChannel(string baseAddress);
}