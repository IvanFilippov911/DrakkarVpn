namespace DrakkarVpn.Shared.InstanceProvider;

public sealed class InstanceIdProvider : IInstanceIdProvider
{
    private readonly string _instanceId;

    public InstanceIdProvider()
    {
        var env =
            Environment.GetEnvironmentVariable("DRAKKAR_INSTANCE_ID")
            ?? Environment.GetEnvironmentVariable("POD_NAME")          
            ?? Environment.GetEnvironmentVariable("HOSTNAME");         

        if (!string.IsNullOrWhiteSpace(env))
        {
            _instanceId = env.Trim();
            return;
        }
        
        var pid = Environment.ProcessId; 
        var rnd = Guid.NewGuid().ToString("N")[..8];
        _instanceId = $"{Environment.MachineName}:{pid}:{rnd}";
    }

    public string GetInstanceId() => _instanceId;
}