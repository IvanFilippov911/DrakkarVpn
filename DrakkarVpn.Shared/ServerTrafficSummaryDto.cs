namespace DrakkarVpn.Shared;

public sealed record ServerTrafficSummaryDto(
    long TrafficLast1hBytes,
    long TrafficLast24hBytes
);