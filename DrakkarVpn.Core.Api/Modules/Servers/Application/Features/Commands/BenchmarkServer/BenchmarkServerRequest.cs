using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.BenchmarkServer;

public sealed record BenchmarkServerRequest(Guid ServerId) : IRequest<BenchmarkResultDto>;