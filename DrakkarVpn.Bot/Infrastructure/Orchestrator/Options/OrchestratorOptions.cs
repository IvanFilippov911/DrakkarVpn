namespace DrakkarVpn.Bot.Infrastructure.Telegram.Options;

public class OrchestratorOptions
{
    public const string SectionName = "Orchestrator";
    public string BaseUrl { get; set; } = string.Empty;
    
    public string RegisterUserEndpoint { get; set; } = "/api/orchestrator/register-or-get";
    public string RegionsEndpoint { get; set; } = "/api/orchestrator/regions";
    public string AllocatePeerEndpoint { get; set; } = "/api/orchestrator/allocate";
    public string GetPeersEndpoint {get; set;} = "/api/orchestrator/peers";
}