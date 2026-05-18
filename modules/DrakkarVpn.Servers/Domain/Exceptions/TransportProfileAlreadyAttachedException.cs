namespace DrakkarVpn.Servers.Domain.Exceptions;

public sealed class TransportProfileAlreadyAttachedException : InvalidOperationException
{
    public TransportProfileAlreadyAttachedException(Guid transportProfileId)
        : base($"Transport profile '{transportProfileId}' is already attached to the server.")
    {
        TransportProfileId = transportProfileId;
    }

    public Guid TransportProfileId { get; }
}
