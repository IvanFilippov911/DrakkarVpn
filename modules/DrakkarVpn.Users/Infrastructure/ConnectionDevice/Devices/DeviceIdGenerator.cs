using System.Security.Cryptography;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions;

namespace DrakkarVpn.Users.Infrastructure.Devices;

public sealed class DeviceIdGenerator : IDeviceIdGenerator
{
    public string Generate() =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(12)).ToLowerInvariant();
}
