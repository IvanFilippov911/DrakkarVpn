using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Idempotency.Infrastructure.EF;

public sealed class IdempotencyDbContextFactory : IDesignTimeDbContextFactory<IdempotencyDbContext>
{
    public IdempotencyDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Db")
            ?? "Host=127.0.0.1;Port=5432;Database=drakkar;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<IdempotencyDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new IdempotencyDbContext(options);
    }
}