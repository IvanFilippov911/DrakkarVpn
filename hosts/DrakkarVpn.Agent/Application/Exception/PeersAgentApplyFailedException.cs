namespace DrakkarVpn.Agent.Application.Exception;

public sealed class PeersAgentApplyFailedException : System.Exception
{
    public string Code { get; }

    public PeersAgentApplyFailedException(string code, string message)
        : base(message)
    {
        Code = string.IsNullOrWhiteSpace(code)
            ? "AGENT_APPLY_FAILED"
            : code;
    }

    public PeersAgentApplyFailedException(
        string code,
        string message,
        System.Exception inner)
        : base(message, inner)
    {
        Code = string.IsNullOrWhiteSpace(code)
            ? "AGENT_APPLY_FAILED"
            : code;
    }
}