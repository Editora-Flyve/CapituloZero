using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Users.AddTipos;
using CapituloZero.IdentityTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace CapituloZero.IdentityTests.Users;

public class AddTiposTests
{
    [Fact]
    public async Task AddTipos_Self_Assign_Default_Autor_Parceiro()
    {
        using var provider = TestHost.Build();
        var identity = provider.GetRequiredService<IIdentityService>();
        var handler = provider.GetRequiredService<CapituloZero.Application.Abstractions.Messaging.ICommandHandler<AddTiposCommand>>();
        var userCtx = provider.GetRequiredService<IUserContext>();

        var reg = await identity.RegisterAsync("diana@test.com", "Diana", "Prince", "abc123");
        reg.IsSuccess.ShouldBeTrue(reg.Error?.Description);

        // Simulate same-user context; our handler compares UserId vs CurrentUserId
        var cmd = new AddTiposCommand(reg.Value, new []{"Autor","Parceiro"}, reg.Value);
        var result = await handler.Handle(cmd, CancellationToken.None);
        result.IsSuccess.ShouldBeTrue(result.Error?.Description);

        var get = await identity.GetByIdAsync(reg.Value, reg.Value);
        get.IsSuccess.ShouldBeTrue(get.Error?.Description);
        get.Value.Tipos.ShouldContain("Default");
        get.Value.Tipos.ShouldContain("Autor");
        get.Value.Tipos.ShouldContain("Parceiro");
    }
}
