using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.IdentityTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace CapituloZero.IdentityTests.Users;

public class IdentityFlowTests
{
    [Fact]
    public async Task RegisterAndLoginSucceedsWithDefaultRole()
    {
        await using var provider = TestHost.Build();
        var identity = provider.GetRequiredService<IIdentityService>();

        var userId = await identity.RegisterAsync("john.doe@test.com", "John", "Doe", "Abc123!").ConfigureAwait(false);
        userId.IsSuccess.ShouldBeTrue(userId.ErrorInternal?.Description);
        userId.Value.ShouldNotBe(Guid.Empty);

        var token = await identity.LoginAsync("john.doe@test.com", "Abc123!").ConfigureAwait(false);
        token.IsSuccess.ShouldBeTrue(token.ErrorInternal?.Description);
        token.Value.AccessToken.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetByIdFailsForDifferentUser()
    {
        await using var provider = TestHost.Build();
        var identity = provider.GetRequiredService<IIdentityService>();

        var userId = await identity.RegisterAsync("alice@test.com", "Alice", "Doe", "Abc123!").ConfigureAwait(false);
        userId.IsSuccess.ShouldBeTrue(userId.ErrorInternal?.Description);

        var otherId = Guid.NewGuid();
        var got = await identity.GetByIdAsync(userId.Value, otherId).ConfigureAwait(false);
        got.IsFailure.ShouldBeTrue();
        got.ErrorInternal.Code.ShouldBe("Users.Unauthorized");
    }
}
