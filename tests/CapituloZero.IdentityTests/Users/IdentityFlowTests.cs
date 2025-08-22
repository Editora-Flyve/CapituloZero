using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.IdentityTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace CapituloZero.IdentityTests.Users;

public class IdentityFlowTests
{
    [Fact]
    public async Task Register_And_Login_Succeeds_With_Default_Role()
    {
        using var provider = TestHost.Build();
        var identity = provider.GetRequiredService<IIdentityService>();

    var userId = await identity.RegisterAsync("john.doe@test.com", "John", "Doe", "abc123");
    userId.IsSuccess.ShouldBeTrue(userId.Error?.Description);
        userId.Value.ShouldNotBe(Guid.Empty);

        var token = await identity.LoginAsync("john.doe@test.com", "abc123");
    token.IsSuccess.ShouldBeTrue(token.Error?.Description);
        token.Value.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetById_Fails_For_Different_User()
    {
        using var provider = TestHost.Build();
        var identity = provider.GetRequiredService<IIdentityService>();

    var userId = await identity.RegisterAsync("alice@test.com", "Alice", "Doe", "abc123");
    userId.IsSuccess.ShouldBeTrue(userId.Error?.Description);

        var otherId = Guid.NewGuid();
        var got = await identity.GetByIdAsync(userId.Value, otherId);
        got.IsFailure.ShouldBeTrue();
    got.Error.Code.ShouldBe("Users.Unauthorized");
    }
}
