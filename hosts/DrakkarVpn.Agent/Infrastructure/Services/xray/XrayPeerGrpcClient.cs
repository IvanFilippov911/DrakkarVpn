using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Agent.Application.Exception;
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

    public async Task RegisterPeerAsync(Guid peerUuid, CancellationToken ct)
    {
        var client = CreateHandlerClient();

        var vless = new Account
        {
            Id = peerUuid.ToString(),
            Flow = "",
            Encryption = "",
            XorMode = 0,
            Seconds = 0,
            Padding = ""
        };

        var typedAccount = new TypedMessage
        {
            Type = "xray.proxy.vless.Account",
            Value = vless.ToByteString()
        };

        var addUser = new AddUserOperation
        {
            User = new User
            {
                Email = $"{peerUuid}@drakkar.local",
                Account = typedAccount
            }
        };

        var request = new AlterInboundRequest
        {
            Tag = _options.InboundTag,
            Operation = new TypedMessage
            {
                Type = "xray.app.proxyman.command.AddUserOperation",
                Value = addUser.ToByteString()
            }
        };

        try
        {
            await client.AlterInboundAsync(request, cancellationToken: ct);
        }
        catch (RpcException ex) when (IsAlreadyExists(ex))
        {
            throw new PeersAgentAlreadyExistsException();
        }
        catch (RpcException ex)
        {
            throw new PeersAgentApplyFailedException(
                code: $"XRAY_{ex.StatusCode}",
                message: ex.Status.Detail ?? ex.Message);
        }
        catch (Exception ex)
        {
            throw new PeersAgentApplyFailedException("XRAY_UNKNOWN", ex.Message);
        }
    }

    private static bool IsAlreadyExists(RpcException ex)
    {
        if (ex.StatusCode == StatusCode.AlreadyExists)
            return true;

        var msg = ex.Status.Detail ?? ex.Message ?? string.Empty;
        return msg.Contains("already", StringComparison.OrdinalIgnoreCase)
               && msg.Contains("exist", StringComparison.OrdinalIgnoreCase);
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
            Tag = _options.InboundTag,
            Operation = new TypedMessage
            {
                Type = "xray.app.proxyman.command.RemoveUserOperation",
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
        var client = CreateHandlerClient();

        var response = await client.GetInboundUsersAsync(new GetInboundUserRequest
        {
            Tag = _options.InboundTag
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