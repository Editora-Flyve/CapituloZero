using System.Text;
using CapituloZero.IdentityTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace CapituloZero.IdentityTests.Users;

public class AddTiposAndGetCurrentTests
{
    private static readonly string[] AutorParceiro = new[] { "Autor", "Parceiro" };
    [Fact]
    public async Task AddTiposSelfAssignAndGetCurrentReturnsTipos()
    {
        using var testHost = TestHost.Build();
        var identity = testHost.GetRequiredService<CapituloZero.Application.Abstractions.Authentication.IIdentityService>();

        // Register and login
        var reg = await identity.RegisterAsync("bruce@test.com", "Bruce", "Wayne", "abc123");
        reg.IsSuccess.ShouldBeTrue(reg.Error?.Description);
        var login = await identity.LoginAsync("bruce@test.com", "abc123");
        login.IsSuccess.ShouldBeTrue(login.Error?.Description);

    // Use IdentityService directly to add tipos
        // Note: our test IUserContext reads from HttpContext in real app; in unit tests we can bypass and call service directly
    var result = await identity.AddUserTypesAsync(reg.Value, AutorParceiro, CancellationToken.None);
        result.IsSuccess.ShouldBeTrue(result.Error?.Description);

        // Query user and assert tipos
        var get = await identity.GetByIdAsync(reg.Value, reg.Value);
        get.IsSuccess.ShouldBeTrue(get.Error?.Description);
        get.Value.Tipos.ShouldContain("Default");
        get.Value.Tipos.ShouldContain("Autor");
        get.Value.Tipos.ShouldContain("Parceiro");
    }
}
