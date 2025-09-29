namespace DrakkarVpn.Bot.Infrastructure.Telegram.Exceptions;

public class OrchestratorException : Exception
{
    public OrchestratorException(string message) : base(message) { }
    public OrchestratorException(string message, Exception inner) : base(message, inner) { }
}