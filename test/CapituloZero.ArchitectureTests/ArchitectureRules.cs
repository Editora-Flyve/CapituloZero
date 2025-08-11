using NetArchTest.Rules;
using Shouldly;
using CapituloZero.Domain.Users.Entities;
using CapituloZero.Application;
using CapituloZero.Web.Api;
using CapituloZero.Infrastructure;

namespace CapituloZero.ArchitectureTests;

public class ArchitectureRules
{
    private const string ApplicationNamespace = "CapituloZero.Application";
    private const string InfrastructureNamespace = "CapituloZero.Infrastructure";
    private const string WebApiNamespace = "CapituloZero.Web.Api";

    [Fact]
    public void DomainShouldNotDependOnApplicationInfrastructureOrWeb()
    {
        var result = Types.InAssembly(typeof(User).Assembly)
            .Should().NotHaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, WebApiNamespace)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(result.GetFailingTypesAsString());
    }

    [Fact]
    public void ApplicationShouldNotDependOnWebApi()
    {
        var result = Types.InAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly)
            .Should().NotHaveDependencyOnAny(WebApiNamespace)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(result.GetFailingTypesAsString());
    }

    [Fact]
    public void InfrastructureShouldNotDependOnWebApi()
    {
        var result = Types.InAssembly(typeof(InfrastructureServiceCollectionExtensions).Assembly)
            .Should().NotHaveDependencyOn(WebApiNamespace)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(result.GetFailingTypesAsString());
    }
}

internal static class FailExtensions
{
    public static string GetFailingTypesAsString(this NetArchTest.Rules.TestResult result)
    {
        if (result.IsSuccessful) return string.Empty;
        return string.Join("\n", result.FailingTypes.Select(t => t.FullName));
    }
}
