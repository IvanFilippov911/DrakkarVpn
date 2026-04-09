using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Admin.Api.Infrastructure.EF.ReadEntities;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.DTOs;
using DrakkarVpn.Users.Application.DTOs.Admin;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Admin.Api.Infrastructure.EF.Repository.ReadRepositories;

public sealed class UserDevicesReadStore : IUserDevicesReadStore
{
    private readonly AdminReadDbContext _db;

    public UserDevicesReadStore(AdminReadDbContext db) => _db = db;

    public async Task<IReadOnlyList<UserDeviceShortDto>> GetDevicesAsync(Guid userId, CancellationToken ct)
    {
        var rows = await (
            from d in _db.Devices
            where d.UserId == userId
            join p in _db.Peers.Where(p => p.Status == (int)PeerStatus.Active)
                on d.DeviceId equals p.DeviceId into peerJoin
            from p in peerJoin.DefaultIfEmpty()
            join agg in _db.AdminPeerTrafficAggs on p.Id equals agg.PeerId into aggJoin
            from agg in aggJoin.DefaultIfEmpty()
            orderby d.CreatedAtUtc descending
            select new { Device = d, Peer = p, Agg = agg }
        ).ToListAsync(ct);

        return rows.Select(row =>
        {
            AdminUserPeerShortDto? peerDto = null;

            if (row.Peer != null)
            {
                peerDto = new AdminUserPeerShortDto(
                    PeerId: row.Peer.Id,
                    ServerId: row.Peer.ServerId,
                    AgentPeerUuid: row.Peer.AgentPeerUuid,
                    IsOnline: row.Peer.IsOnline,
                    Traffic24hBytes: row.Agg?.Last24hBytes ?? 0
                );
            }

            return new UserDeviceShortDto(
                DeviceId: row.Device.DeviceId,
                Name: row.Device.Name,
                Platform: row.Device.Platform,
                CreatedAtUtc: row.Device.CreatedAtUtc,
                LastSeenUtc: row.Device.LastSeenUtc,
                Status: row.Device.Status,
                Peer: peerDto
            );
        }).ToList();
    }
}