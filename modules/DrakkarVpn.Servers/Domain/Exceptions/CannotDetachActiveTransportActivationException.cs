namespace DrakkarVpn.Servers.Domain.Exceptions;

public sealed class CannotDetachActiveTransportActivationException : InvalidOperationException
{
    public CannotDetachActiveTransportActivationException(Guid activationId)
        : base($"Cannot detach active transport activation '{activationId}'.")
    {
        ActivationId = activationId;
    }

    public Guid ActivationId { get; }
}
