using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CapituloZero.Infrastructure.Database;
using CapituloZero.Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.ApplicationTests.Helpers;

public static class TestDb
{
    public static ApplicationDbContext CreateContext(IDomainEventsDispatcher? dispatcher = null)
    {
        dispatcher ??= new NoopDomainEventsDispatcher();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"app-tests-{Guid.NewGuid()}")
            .EnableSensitiveDataLogging()
            .Options;
        return new ApplicationDbContext(options, dispatcher);
    }
}

public sealed class NoopDomainEventsDispatcher : IDomainEventsDispatcher
{
    public Task DispatchAsync(IEnumerable<CapituloZero.SharedKernel.IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

public sealed class SpyDomainEventsDispatcher : IDomainEventsDispatcher
{
    public List<CapituloZero.SharedKernel.IDomainEvent> Events { get; } = new();

    public Task DispatchAsync(IEnumerable<CapituloZero.SharedKernel.IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        Events.AddRange(domainEvents);
        return Task.CompletedTask;
    }
}

