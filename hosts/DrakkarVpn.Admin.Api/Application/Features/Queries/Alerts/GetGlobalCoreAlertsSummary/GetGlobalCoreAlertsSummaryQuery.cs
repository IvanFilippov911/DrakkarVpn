using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetGlobalCoreAlertsSummary;

public sealed record GetGlobalCoreAlertsSummaryQuery
    : IRequest<CoreAlertsGlobalSummaryDto>;