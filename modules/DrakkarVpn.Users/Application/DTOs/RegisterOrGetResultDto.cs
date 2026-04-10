using DrakkarVpn.Core.Api.Modules.Users.Domain;

namespace DrakkarVpn.Users.Application.DTOs;

public sealed record RegisterOrGetResultDto(bool IsNew, AppUser User);