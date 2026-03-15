using DrakkarVpn.AdminAuth.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.AdminAuth.Infrastructure.EF.Configurations;

internal sealed class AdminIdentityRoleConfiguration : IEntityTypeConfiguration<AdminIdentityRole>
{
    public void Configure(EntityTypeBuilder<AdminIdentityRole> b)
    {
        b.ToTable("admin_roles");

        b.Property(x => x.Id)
            .HasColumnName("id");

        b.Property(x => x.Name)
            .HasColumnName("name");

        b.Property(x => x.NormalizedName)
            .HasColumnName("normalized_name");

        b.Property(x => x.ConcurrencyStamp)
            .HasColumnName("concurrency_stamp");
    }
}
