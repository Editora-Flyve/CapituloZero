using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.IdentityTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace CapituloZero.IdentityTests.Users;

public class GetCurrentTests
{
    [Fact]
    public async Task GetCurrent_Returns_User_With_Default_Tipo()
    {
        using var provider = TestHost.Build();
        var identity = provider.GetRequiredService<IIdentityService>();

        var reg = await identity.RegisterAsync("clark@test.com", "Clark", "Kent", "abc123");
        reg.IsSuccess.ShouldBeTrue(reg.Error?.Description);

        var me = await identity.GetByIdAsync(reg.Value, reg.Value);
        me.IsSuccess.ShouldBeTrue(me.Error?.Description);
        me.Value.Email.ShouldBe("clark@test.com");
        me.Value.Tipos.ShouldContain("Default");
    }
}
