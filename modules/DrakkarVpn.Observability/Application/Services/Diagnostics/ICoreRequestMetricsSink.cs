namespace DrakkarVpn.Core.Api.Diagnostics;

public interface ICoreRequestMetricsSink
{
    void TrackSuccess(string command, long elapsedMs);
    void TrackError(string command, long elapsedMs, string? area = null);
}