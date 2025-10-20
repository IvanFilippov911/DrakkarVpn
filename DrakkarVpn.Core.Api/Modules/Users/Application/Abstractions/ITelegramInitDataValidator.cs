namespace DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;

public interface ITelegramInitDataValidator
{
    (long TelegramId, long AuthDateUnix, string? QueryId) ValidateAndExtractAll(string initData);             
}