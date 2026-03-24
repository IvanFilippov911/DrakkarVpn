namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

public enum HomeScreenStateDto : short
{
    Blocked        = 0,
    NoSubscription = 1,
    NotStarted     = 2,
    Pending        = 3,
    Ready          = 4
}

