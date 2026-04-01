namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Errors;

public static class PeerProvisionErrorCodes
{
    public const string ServerNotLoaded          = "ServerNotLoaded";

    // Agent call layer (transport/exception/etc)
    public const string AgentCallFailed          = "AgentCallFailed";
    public const string AgentTimeout             = "AgentTimeout";

    // Agent response validation / consistency
    public const string AgentResultInvalid       = "AgentResultInvalid";
    public const string AgentResultMissing       = "AgentResultMissing";

    // Job checkpointing
    public const string JobCheckpointWriteFailed = "JobCheckpointWriteFailed";
    public const string JobCheckpointInvalid     = "JobCheckpointInvalid";
    public const string DomainPeerCreationFailed = "DomainPeerCreationFailed";
    public const string DomainDbConflictTargetMismatch = "DomainDbConflictTargetMismatch";
    public const string DomainDbUniqueViolation = "DomainDbUniqueViolation";
    public const string DomainDbWriteFailed = "DomainDbWriteFailed";
    
    public const string AgentApplyFailed = "AgentApplyFailed";
    
    public const string JobPayloadInvalid = "JobPayloadInvalid";
    
    
    
    
}