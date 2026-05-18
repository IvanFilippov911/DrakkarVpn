using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Servers.Application.Options;

internal sealed class ServerTransportAgentApplyOptionsValidator : IValidateOptions<ServerTransportAgentApplyOptions>
{
    private readonly IConfiguration _configuration;

    public ServerTransportAgentApplyOptionsValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ValidateOptionsResult Validate(string? name, ServerTransportAgentApplyOptions options)
    {
        if (!ServerTransportApplyOptionsValidation.TryValidateAgentOptions(options, out var agentMessage))
            return ValidateOptionsResult.Fail(agentMessage);

        var job = _configuration
            .GetSection(ServerTransportApplyJobOptions.SectionName)
            .Get<ServerTransportApplyJobOptions>() ?? new ServerTransportApplyJobOptions();

        if (!ServerTransportApplyOptionsValidation.TryValidateCombined(job, options, out var combinedMessage))
            return ValidateOptionsResult.Fail(combinedMessage);

        return ValidateOptionsResult.Success;
    }
}
