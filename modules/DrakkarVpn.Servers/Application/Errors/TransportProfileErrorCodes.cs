namespace DrakkarVpn.Servers.Application.Errors;

public static class TransportProfileErrorCodes
{
    public const string NameAlreadyExists = "TRANSPORT_PROFILE_NAME_ALREADY_EXISTS";
    public const string UnsupportedSecurityType = "TRANSPORT_PROFILE_UNSUPPORTED_SECURITY_TYPE";
    public const string GrpcServiceNameRequired = "TRANSPORT_PROFILE_GRPC_SERVICE_NAME_REQUIRED";
}
