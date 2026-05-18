using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Servers.Application.Options;

internal sealed class ServerTransportApplyJobOptionsValidator : IValidateOptions<ServerTransportApplyJobOptions>
{
    private readonly IConfiguration _configuration;

    public ServerTransportApplyJobOptionsValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ValidateOptionsResult Validate(string? name, ServerTransportApplyJobOptions options)
    {
        if (!ServerTransportApplyOptionsValidation.TryValidateJobOptions(options, out var jobMessage))
            return ValidateOptionsResult.Fail(jobMessage);

        var agent = _configuration
            .GetSection(ServerTransportAgentApplyOptions.SectionName)
            .Get<ServerTransportAgentApplyOptions>() ?? new ServerTransportAgentApplyOptions();

        if (!ServerTransportApplyOptionsValidation.TryValidateCombined(options, agent, out var combinedMessage))
            return ValidateOptionsResult.Fail(combinedMessage);

        return ValidateOptionsResult.Success;
    }
}
