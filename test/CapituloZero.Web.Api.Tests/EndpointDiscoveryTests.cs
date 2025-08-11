using System.Reflection;
using CapituloZero.Web.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CapituloZero.Web.Api.Tests;

public class EndpointDiscoveryTests
{
    [Fact]
    public void AddEndpoints_Registers_All_IEndpoints_In_Assembly()
    {
        var services = new ServiceCollection();
        services.AddEndpoints(Assembly.Load("CapituloZero.Web.Api"));
        var provider = services.BuildServiceProvider();

        var endpoints = provider.GetRequiredService<IEnumerable<CapituloZero.Web.Api.Endpoints.IEndpoint>>();
        Assert.NotEmpty(endpoints);
    }
}
