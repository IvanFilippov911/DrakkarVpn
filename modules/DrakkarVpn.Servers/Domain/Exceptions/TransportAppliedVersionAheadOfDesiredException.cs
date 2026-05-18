namespace DrakkarVpn.Servers.Domain.Exceptions;

public sealed class TransportAppliedVersionAheadOfDesiredException : InvalidOperationException
{
    public TransportAppliedVersionAheadOfDesiredException(long incomingVersion, long desiredVersion)
        : base($"Applied version '{incomingVersion}' is ahead of desired version '{desiredVersion}'.")
    {
        IncomingVersion = incomingVersion;
        DesiredVersion = desiredVersion;
    }

    public long IncomingVersion { get; }
    public long DesiredVersion { get; }
}
