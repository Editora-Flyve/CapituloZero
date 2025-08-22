using System.Text;
using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Infrastructure.Authentication;
using CapituloZero.Infrastructure.Database;
using CapituloZero.Infrastructure.Usuarios;
using CapituloZero.Infrastructure.DomainEvents;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CapituloZero.IdentityTests.Helpers;

internal static class TestHost
{
    public static ServiceProvider Build()
    {
        var services = new ServiceCollection();

        // Configuration with Jwt
        var inMemoryConfig = new Dictionary<string, string?>
        {
            ["Jwt:Secret"] = Convert.ToBase64String(Encoding.UTF8.GetBytes("test-secret-12345678901234567890")),
            ["Jwt:Issuer"] = "test-issuer",
            ["Jwt:Audience"] = "test-audience",
            ["Jwt:ExpirationInMinutes"] = "60"
        };
        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemoryConfig!).Build();
        services.AddSingleton(config);

        // EF Core InMemory
        services.AddDbContext<ApplicationDbContext>(o =>
            o.UseInMemoryDatabase($"identity-tests-{Guid.NewGuid()}")
             .EnableSensitiveDataLogging());

        // Identity Core with EF stores
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager();

        // Abstractions
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IIdentityService, IdentityService>();
    services.AddScoped<IDomainEventsDispatcher, NoopDomainEventsDispatcher>();

        return services.BuildServiceProvider();
    }
}

internal sealed class NoopDomainEventsDispatcher : IDomainEventsDispatcher
{
    public Task DispatchAsync(IEnumerable<CapituloZero.SharedKernel.IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    => Task.CompletedTask;
}
