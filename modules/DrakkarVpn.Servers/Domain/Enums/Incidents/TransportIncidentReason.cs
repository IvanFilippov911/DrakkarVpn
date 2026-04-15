namespace DrakkarVpn.Servers.Domain.Enums.Incidents;

public enum TransportIncidentReason
{
    ProbeFailureThresholdExceeded = 1,
    AllStandbyProfilesFailed = 2,
    ManualEscalation = 3,
    ManualRecovery = 4
}