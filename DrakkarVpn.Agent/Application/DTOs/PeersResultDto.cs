namespace DrakkarVpn.Agent.Application.DTOs;

public sealed record PeersResultDto(Guid Uuid, string Email, string Flow, string Encryption);