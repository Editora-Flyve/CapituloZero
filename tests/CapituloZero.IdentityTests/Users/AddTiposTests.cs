using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Users.AddTipos;
using CapituloZero.IdentityTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace CapituloZero.IdentityTests.Users;

public class AddTiposTests
{
    private static readonly string[] AutorParceiro = new[] { "Autor", "Parceiro" };

    [Fact]
    public async Task AddTiposSelfAssignDefaultAutorParceiro()
    {
        using var provider = TestHost.Build();
        var identity = provider.GetRequiredService<IIdentityService>();

        var reg = await identity.RegisterAsync("diana@test.com", "Diana", "Prince", "abc123");
        reg.IsSuccess.ShouldBeTrue(reg.Error?.Description);

        // Use identity service directly
        var result = await identity.AddUserTypesAsync(reg.Value, AutorParceiro, CancellationToken.None);
        result.IsSuccess.ShouldBeTrue(result.Error?.Description);

        var get = await identity.GetByIdAsync(reg.Value, reg.Value);
        get.IsSuccess.ShouldBeTrue(get.Error?.Description);
        get.Value.Tipos.ShouldContain("Default");
        get.Value.Tipos.ShouldContain("Autor");
        get.Value.Tipos.ShouldContain("Parceiro");
    }
}
