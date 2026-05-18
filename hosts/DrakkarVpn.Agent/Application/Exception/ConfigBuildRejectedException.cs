namespace DrakkarVpn.Agent.Application.Exception;

public sealed class ConfigBuildRejectedException : System.Exception
{
    public ConfigBuildRejectedException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
