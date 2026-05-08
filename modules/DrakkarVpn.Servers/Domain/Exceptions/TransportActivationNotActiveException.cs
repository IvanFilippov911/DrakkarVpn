namespace DrakkarVpn.Core.Api.Modules.Servers.Domain.Exceptions;

public sealed class TransportActivationNotActiveException : InvalidOperationException
{
    public TransportActivationNotActiveException(Guid activationId)
        : base($"Transport activation '{activationId}' is not in active status.")
    {
        ActivationId = activationId;
    }

    public Guid ActivationId { get; }
}
