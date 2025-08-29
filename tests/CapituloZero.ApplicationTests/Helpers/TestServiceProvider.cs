using CapituloZero.Application;
using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Infrastructure.Database;
using CapituloZero.Infrastructure.DomainEvents;
using CapituloZero.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CapituloZero.ApplicationTests.Helpers;

internal static class TestServiceProvider
{
    public static ServiceProvider Build(
        IDomainEventsDispatcher? dispatcher = null,
        Guid? currentUserId = null,
        DateTime? now = null,
        string? dbName = null)
    {
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddLogging();

        dispatcher ??= Substitute.For<IDomainEventsDispatcher>();
        dbName ??= $"tests-{Guid.NewGuid()}";

        services.AddDbContext<ApplicationDbContext>(o =>
            o.UseInMemoryDatabase(dbName)
             .EnableSensitiveDataLogging());

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUserContext>(_ => new FakeUserContext { UserId = currentUserId ?? Guid.NewGuid() });
        services.AddScoped<IDateTimeProvider>(_ => new StubDateTimeProvider { UtcNow = now ?? DateTime.UtcNow });
        services.AddSingleton(dispatcher);

        return services.BuildServiceProvider();
    }
}
