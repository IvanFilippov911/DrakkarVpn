using System.Text.Json.Serialization;

namespace DrakkarVpn.Shared.Users;

public sealed record RegisterUserRequest([property: JsonPropertyName("telegramId")]long TelegramId);