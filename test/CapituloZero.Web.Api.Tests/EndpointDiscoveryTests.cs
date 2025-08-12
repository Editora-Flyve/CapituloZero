using System.Reflection;
using CapituloZero.Web.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CapituloZero.Web.Api.Tests;

public class EndpointDiscoveryTests
{
    [Fact]
    public void AddEndpointsRegistersAllIEndpointsInAssembly()
    {
    var services = new ServiceCollection();
    services.AddEndpoints(typeof(CapituloZero.Web.Api.Program).Assembly);
        var provider = services.BuildServiceProvider();

        var endpoints = provider.GetRequiredService<IEnumerable<CapituloZero.Web.Api.Endpoints.IEndpoint>>();
        Assert.NotEmpty(endpoints);
    }
}
