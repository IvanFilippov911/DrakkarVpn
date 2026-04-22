namespace DrakkarVpn.Servers.Application.Errors;

public static class ServerTransportActivationErrorCodes
{
    public const string ServerNotFound = "SERVER_TRANSPORT_ACTIVATION_SERVER_NOT_FOUND";
    public const string ActivationNotFound = "SERVER_TRANSPORT_ACTIVATION_NOT_FOUND";
    public const string DuplicateTransportProfileIdsInInput = "SERVER_TRANSPORT_ACTIVATION_DUPLICATE_PROFILE_IDS_IN_INPUT";
    public const string ActivateProfileIdIsNotInInput = "SERVER_TRANSPORT_ACTIVATION_ACTIVATE_PROFILE_ID_NOT_IN_INPUT";
    public const string TransportProfileNotFound = "SERVER_TRANSPORT_ACTIVATION_PROFILE_NOT_FOUND";
    public const string TransportProfileDisabled = "SERVER_TRANSPORT_ACTIVATION_PROFILE_DISABLED";
    public const string TransportProfileAlreadyAttached = "SERVER_TRANSPORT_ACTIVATION_PROFILE_ALREADY_ATTACHED";
    public const string ActiveActivationCannotBeDetached = "SERVER_TRANSPORT_ACTIVATION_ACTIVE_CANNOT_BE_DETACHED";
}
