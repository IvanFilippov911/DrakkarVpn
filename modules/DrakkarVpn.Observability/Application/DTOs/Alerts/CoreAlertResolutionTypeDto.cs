namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record CoreAlertResolutionTypeDto(
    string Code,        
    string Label,       
    string Description  
);