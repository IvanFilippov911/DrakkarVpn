namespace DrakkarVpn.Shared.Errors;

public sealed class UserBannedException : Exception
{
    public Guid UserId { get; }

    public UserBannedException(Guid userId, string? message = null)
        : base(message ?? "User is banned")
    {
        UserId = userId;
    }
}