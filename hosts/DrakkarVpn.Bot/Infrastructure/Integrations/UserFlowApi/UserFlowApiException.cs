namespace DrakkarVpn.Bot.Infrastructure.Integrations.UserFlowApi.Exceptions;

public sealed class UserFlowApiException : Exception
{
    public UserFlowApiException(string message) : base(message) { }

    public UserFlowApiException(string message, Exception inner) : base(message, inner) { }
}