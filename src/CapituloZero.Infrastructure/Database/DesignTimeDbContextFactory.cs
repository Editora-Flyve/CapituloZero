using CapituloZero.Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CapituloZero.Infrastructure.Database;

internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Use a local dev connection string or environment-provided one; default to localhost
    string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__postgresdb")
            ?? "Host=localhost;Port=5432;Database=capitulo_zero_dev;Username=postgres;Password=postgres";

        optionsBuilder
            .UseNpgsql(connectionString, npgsqlOptions =>
                npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
            .UseSnakeCaseNamingConvention();

        // Use a no-op domain events dispatcher for design-time
        var noOpDispatcher = new NoOpDomainEventsDispatcher();

        return new ApplicationDbContext(optionsBuilder.Options, noOpDispatcher);
    }

    private sealed class NoOpDomainEventsDispatcher : IDomainEventsDispatcher
    {
        public Task DispatchAsync(IEnumerable<SharedKernel.IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
