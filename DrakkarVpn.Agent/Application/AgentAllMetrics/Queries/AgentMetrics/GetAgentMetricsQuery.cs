using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Agent.Application.Metrics.Queries.AgentMetrics;

public sealed record GetAgentMetricsQuery() : IRequest<AgentMetricsDto>;