using DrakkarVpn.Agent.Infrastructure.EF.Configurations;
using DrakkarVpn.Agent.Infrastructure.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Agent.Infrastructure.EF;

public sealed class AgentDbContext : DbContext
{
    public AgentDbContext(DbContextOptions<AgentDbContext> options)
        : base(options)
    {
    }

    public DbSet<AgentTransportState> AgentTransportStates => Set<AgentTransportState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AgentTransportStateConfiguration());
    }
}
