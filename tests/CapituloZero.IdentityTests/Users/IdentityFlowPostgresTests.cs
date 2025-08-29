using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.IdentityTests.Helpers;
using CapituloZero.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace CapituloZero.IdentityTests.Users;

[Collection("postgres")]
public class IdentityFlowPostgresTests
{
    private readonly PostgresContainerFixture _fx;

    public IdentityFlowPostgresTests(PostgresContainerFixture fx)
    {
        _fx = fx;
    }

    [Fact]
    public async Task RegisterAndLoginOnPostgresUsingMigrations()
    {
        if (!_fx.IsReady)
        {
            return; // skip if Docker not available
        }

        // reset DB state for isolation
        await _fx.ResetAsync().ConfigureAwait(false);

        await using var provider = TestHost.Build(_fx.ConnectionString);

        // apply migrations
        using (var scope = provider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync().ConfigureAwait(false);
        }

        var identity = provider.GetRequiredService<IIdentityService>();

        var email = DataGen.Email();
        var pass = "Abc123!";

        var userId = await identity.RegisterAsync(email, DataGen.FirstName(), DataGen.LastName(), pass).ConfigureAwait(false);
        userId.IsSuccess.ShouldBeTrue(userId.ErrorInternal?.Description);
        userId.Value.ShouldNotBe(Guid.Empty);

        var token = await identity.LoginAsync(email, pass).ConfigureAwait(false);
        token.IsSuccess.ShouldBeTrue(token.ErrorInternal?.Description);
        token.Value.AccessToken.ShouldNotBeNullOrEmpty();
    }
}
