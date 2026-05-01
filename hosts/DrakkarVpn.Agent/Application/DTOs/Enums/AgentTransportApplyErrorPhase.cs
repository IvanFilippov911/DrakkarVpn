namespace DrakkarVpn.Agent.Application.DTOs.Enums;

public enum AgentTransportApplyErrorPhase
{
    None,

    FluentValidation,
    Preconditions,

    Hashing,

    StateRead,
    ConfigApply,
    StatePersist,

    Unhandled
}
