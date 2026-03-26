using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs.ProvisionPeers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Validation;

internal static class PeerBatchNormalizer
{
    
    private const int MaxDeviceIdLen = 100;
    private const int MaxConfigRawLen = 100;
    
    public static Dictionary<string, PeerBatchCreateDto> NormalizeAndDedupe(
        IReadOnlyCollection<PeerBatchCreateDto> dtos)
    {
        var map = new Dictionary<string, PeerBatchCreateDto>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var dto in dtos)
        {
            if (dto.JobId == Guid.Empty) continue;
            if (dto.ServerId == Guid.Empty) continue;
            if (dto.AgentPeerUuid == Guid.Empty) continue;

            var deviceId = NormalizeDeviceId(dto.DeviceId);
            if (deviceId.Length == 0) continue;
            

            map[deviceId] = dto with
            {
                DeviceId  = deviceId,
            };
        }

        return map;
    }

    private static string NormalizeDeviceId(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            return string.Empty;

        deviceId = deviceId.Trim();
        if (deviceId.Length > MaxDeviceIdLen)
            deviceId = deviceId[..MaxDeviceIdLen];

        return deviceId;
    }

    private static string? NormalizeConfigRaw(string? configRaw)
    {
        if (string.IsNullOrWhiteSpace(configRaw))
            return null;

        configRaw = configRaw.Trim();
        if (configRaw.Length > MaxConfigRawLen)
            configRaw = configRaw[..MaxConfigRawLen];

        return configRaw;
    }
}