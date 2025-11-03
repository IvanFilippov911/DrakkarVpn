using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Agent.Infrastructure.Config;
using DrakkarVpn.Shared.Peers;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Xray.App.Proxyman.Command;
using Xray.App.Stats.Command;
using Xray.Common.Protocol;
using Xray.Common.Serial;
using Xray.Proxy.Vless;

namespace DrakkarVpn.Agent.Infrastructure.Services.Grpc;

public sealed class XrayPeerGrpcClient : IXrayPeerClient
{
    private readonly IGrpcChannelProvider _channels;
    private readonly XrayOptions _options;

    public XrayPeerGrpcClient(IGrpcChannelProvider channels, IOptions<XrayOptions> options)
    {
        _channels = channels;
        _options = options.Value;
    }

    private GrpcChannel GetChannel()
        => _channels.GetChannel($"http://{_options.LocalApiHost}:{_options.ApiPort}");

    private HandlerService.HandlerServiceClient CreateHandlerClient()
        => new HandlerService.HandlerServiceClient(GetChannel());

    private StatsService.StatsServiceClient CreateStatsClient()
        => new StatsService.StatsServiceClient(GetChannel());

    
    public async Task<RegisterPeerResponseDto> RegisterPeerAsync(CancellationToken ct)
    {
        var peerUuid = Guid.NewGuid();
        var client = CreateHandlerClient();

        var vless = new Account
        {
            Id          = peerUuid.ToString(),
            Flow        = "",
            Encryption  = "",
            XorMode     = 0,
            Seconds     = 0,
            Padding     = ""
        };

        var typedAccount = new TypedMessage
        {
            Type  = "xray.proxy.vless.Account",
            Value = vless.ToByteString()
        };

        var addUser = new AddUserOperation
        {
            User = new User
            {
                Email   = $"{peerUuid}@drakkar.local",
                Account = typedAccount
            }
        };

        var request = new AlterInboundRequest
        {
            Tag       = "vless-in",
            Operation = new TypedMessage
            {
                Type  = "xray.app.proxyman.command.AddUserOperation",
                Value = addUser.ToByteString()
            }
        };

        await client.AlterInboundAsync(request, cancellationToken: ct);

        var vlessUrl =
            $"vless://{peerUuid}@{_options.PublicHost}:{_options.PublicPort}" +
            $"?security=tls&encryption=none&type=tcp&allowInsecure=1#Drakkar-{peerUuid.ToString()[..8]}";

        return new RegisterPeerResponseDto(peerUuid, vlessUrl);
    }

    
    public async Task<bool> RevokePeerAsync(Guid peerUuid, CancellationToken ct)
    {
        var client = CreateHandlerClient();

        var removeUser = new RemoveUserOperation
        {
            Email = $"{peerUuid}@drakkar.local"
        };

        var request = new AlterInboundRequest
        {
            Tag       = "vless-in",
            Operation = new TypedMessage
            {
                Type  = "xray.app.proxyman.command.RemoveUserOperation",
                Value = removeUser.ToByteString()
            }
        };

        try
        {
            var response = await client.AlterInboundAsync(request, cancellationToken: ct);
            return response is not null;
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"[RevokePeer] gRPC error: {ex.StatusCode} - {ex.Status.Detail}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RevokePeer] Unexpected error: {ex.Message}");
            return false;
        }
    }

    
    public async Task<IReadOnlyList<PeersResultDto>> GetListPeersAsync(CancellationToken ct)
    {
        var client   = CreateHandlerClient();
        var response = await client.GetInboundUsersAsync(new GetInboundUserRequest
        {
            Tag = "vless-in"
        }, cancellationToken: ct);

        return response.Users
            .Select(u =>
            {
                try
                {
                    var vless = Account.Parser.ParseFrom(u.Account.Value);

                    return new PeersResultDto(
                        Guid.TryParse(vless.Id, out var id) ? id : Guid.Empty,
                        u.Email,
                        vless.Flow,
                        vless.Encryption
                    );
                }
                catch
                {
                    return new PeersResultDto(Guid.Empty, u.Email, "", "");
                }
            })
            .ToList();
    }
}