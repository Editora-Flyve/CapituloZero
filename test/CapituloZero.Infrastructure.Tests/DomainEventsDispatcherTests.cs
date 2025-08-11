using CapituloZero.Infrastructure.DomainEvents;
using CapituloZero.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace CapituloZero.Infrastructure.Tests;

public class DomainEventsDispatcherTests
{
    private sealed class PingEvent : IDomainEvent { }

    private sealed class PingHandler : IDomainEventHandler<PingEvent>
    {
        public int Count { get; private set; }
        public Task Handle(PingEvent domainEvent, CancellationToken cancellationToken)
        {
            Count++;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task DispatchAsync_Resolves_And_Invokes_Handlers()
    {
        var services = new ServiceCollection();
        services.AddScoped<IDomainEventHandler<PingEvent>, PingHandler>();
        var provider = services.BuildServiceProvider();

        var dispatcher = new DomainEventsDispatcher(provider);
        var ev = new PingEvent();

        await dispatcher.DispatchAsync(new []{ ev });

        var handler = provider.GetRequiredService<IDomainEventHandler<PingEvent>>() as PingHandler;
        Assert.NotNull(handler);
        Assert.Equal(1, handler!.Count);
    }
}
