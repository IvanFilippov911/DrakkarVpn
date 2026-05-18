namespace DrakkarVpn.Servers.Domain.Exceptions;

public sealed class TransportActivationNotAttachedException : InvalidOperationException
{
    public TransportActivationNotAttachedException(Guid activationId)
        : base($"Transport activation '{activationId}' is not attached to the server.")
    {
        ActivationId = activationId;
    }

    public Guid ActivationId { get; }
}
