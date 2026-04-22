using DrakkarVpn.Servers.Application.Abstractions.Services;

namespace DrakkarVpn.Servers.Infrastructure.Time;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
