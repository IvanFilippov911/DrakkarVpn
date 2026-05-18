namespace DrakkarVpn.Agent.Application.DTOs.Enums;

public enum AgentTransportApplyPhase
{
    None,
    Validation,
    Preconditions,
    Hashing,
    StateRead,
    ConfigBuild,
    ConfigWrite,
    XrayReload,
    HealthCheck,
    Rollback,
    StatePersist
}
