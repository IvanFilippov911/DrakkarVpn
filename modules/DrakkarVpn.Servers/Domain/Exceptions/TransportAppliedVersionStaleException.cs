namespace DrakkarVpn.Servers.Domain.Exceptions;

public sealed class TransportAppliedVersionStaleException : InvalidOperationException
{
    public TransportAppliedVersionStaleException(long incomingVersion, long currentAppliedVersion)
        : base($"Applied version '{incomingVersion}' is not greater than current applied version '{currentAppliedVersion}'.")
    {
        IncomingVersion = incomingVersion;
        CurrentAppliedVersion = currentAppliedVersion;
    }

    public long IncomingVersion { get; }
    public long CurrentAppliedVersion { get; }
}
