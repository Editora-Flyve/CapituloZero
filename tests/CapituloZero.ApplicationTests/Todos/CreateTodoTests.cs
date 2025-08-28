using System;
using System.Linq;
using System.Threading.Tasks;
using CapituloZero.Application;
using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Todos.Create;
using CapituloZero.Domain.Todos;
using CapituloZero.Domain.Users;
using CapituloZero.Infrastructure.Database;
using CapituloZero.Infrastructure.Todos;
using CapituloZero.Infrastructure.DomainEvents;
using CapituloZero.SharedKernel;
using CapituloZero.ApplicationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace CapituloZero.ApplicationTests.Todos;

public class CreateTodoTests
{
    private static ServiceProvider BuildProvider(SpyDomainEventsDispatcher? spy = null, Guid? currentUserId = null, DateTime? now = null)
    {
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddLogging();

        // EF Core InMemory
        spy ??= new SpyDomainEventsDispatcher();
        services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase($"todos-create-{Guid.NewGuid()}").EnableSensitiveDataLogging());
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Fakes
        services.AddScoped<IUserContext>(_ => new FakeUserContext { UserId = currentUserId ?? Guid.NewGuid() });
        services.AddScoped<IDateTimeProvider>(_ => new StubDateTimeProvider { UtcNow = now ?? DateTime.UtcNow });
        services.AddSingleton<IDomainEventsDispatcher>(spy);

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Create_succeeds_and_raises_domain_event()
    {
        var currentUser = Guid.NewGuid();
        var now = new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var spy = new SpyDomainEventsDispatcher();
        using var provider = BuildProvider(spy, currentUser, now);
        var db = provider.GetRequiredService<ApplicationDbContext>();
        var handler = provider.GetRequiredService<ICommandHandler<CreateTodoCommand, Guid>>();

        var cmd = new CreateTodoCommand
        {
            UserId = currentUser,
            Description = "Buy milk",
            Priority = Priority.Medium,
            DueDate = now.AddDays(1),
            Labels = ["home", "shopping"]
        };

        var result = await handler.Handle(cmd, default);
        result.IsSuccess.ShouldBeTrue(result.Error?.Description);
        result.Value.ShouldNotBe(Guid.Empty);

        var saved = await db.TodoItems.SingleAsync(t => t.Id == result.Value);
        saved.UserId.ShouldBe((UserId)currentUser);
        saved.Description.ShouldBe("Buy milk");
        saved.CreatedAt.ShouldBe(now);
        saved.IsCompleted.ShouldBeFalse();

        // Domain events dispatched after SaveChanges
        spy.Events.OfType<TodoItemCreatedDomainEvent>().Any(e => e.TodoItemId == saved.Id).ShouldBeTrue();
    }

    [Fact]
    public async Task Create_fails_when_user_context_differs()
    {
        var currentUser = Guid.NewGuid();
        using var provider = BuildProvider(currentUserId: currentUser);
        var handler = provider.GetRequiredService<ICommandHandler<CreateTodoCommand, Guid>>();

        var cmd = new CreateTodoCommand
        {
            UserId = Guid.NewGuid(), // different
            Description = "X",
            Priority = Priority.Low
        };

        var result = await handler.Handle(cmd, default);
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(UserErrors.Unauthorized().Code);
    }

    [Fact]
    public async Task Create_fails_validation_when_description_empty()
    {
        var currentUser = Guid.NewGuid();
        using var provider = BuildProvider(currentUserId: currentUser);
        var handler = provider.GetRequiredService<ICommandHandler<CreateTodoCommand, Guid>>();

        var cmd = new CreateTodoCommand
        {
            UserId = currentUser,
            Description = string.Empty,
            Priority = Priority.Low
        };

        var result = await handler.Handle(cmd, default);
        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.Validation);
    }
}
