using Bogus;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Xunit;

namespace CapituloZero.IdentityTests.Helpers;

public sealed class PostgresContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    private Respawner? _respawner;

    public string ConnectionString => _container.GetConnectionString();
    public bool IsReady { get; private set; }

    public PostgresContainerFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("capitulozero_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _container.StartAsync();

            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            // Initialize Respawn with both default schema and users schema
            _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
            {
                SchemasToInclude = new[] { "public", "users" },
                DbAdapter = DbAdapter.Postgres
            });

            IsReady = true;
        }
        catch
        {
            IsReady = false;
        }
    }

    public async Task ResetAsync()
    {
        if (_respawner is null)
            return;

        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();
        await _respawner.ResetAsync(conn);
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }
}

public static class DataGen
{
    private static readonly Faker Faker = new("pt_BR");

    public static string Email() => Faker.Internet.Email();
    public static string FirstName() => Faker.Name.FirstName();
    public static string LastName() => Faker.Name.LastName();
    public static string Password() => Faker.Internet.Password(8);
}
