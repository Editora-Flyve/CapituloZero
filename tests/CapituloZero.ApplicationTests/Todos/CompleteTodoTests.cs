using System;
using System.Linq;
using System.Threading.Tasks;
using CapituloZero.Application;
using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Todos.Complete;
using CapituloZero.Application.Todos.Create;
using CapituloZero.Domain.Todos;
using CapituloZero.Infrastructure.Database;
using CapituloZero.SharedKernel;
using CapituloZero.ApplicationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace CapituloZero.ApplicationTests.Todos;

public class CompleteTodoTests
{
    private static ServiceProvider BuildProvider(SpyDomainEventsDispatcher? spy = null, Guid? currentUserId = null, DateTime? now = null)
    {
        var services = new ServiceCollection();
        services.AddApplication();

        spy ??= new SpyDomainEventsDispatcher();
        services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase($"todos-complete-{Guid.NewGuid()}").EnableSensitiveDataLogging());
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IUserContext>(_ => new FakeUserContext { UserId = currentUserId ?? Guid.NewGuid() });
        services.AddScoped<IDateTimeProvider>(_ => new StubDateTimeProvider { UtcNow = now ?? DateTime.UtcNow });
        services.AddSingleton<IDomainEventsDispatcher>(spy);

        return services.BuildServiceProvider();
    }

    private static async Task<Guid> SeedTodo(ApplicationDbContext db, Guid userId, DateTime createdAt)
    {
        var todo = new TodoItem
        {
            UserId = userId,
            Description = "Task",
            Priority = Priority.High,
            CreatedAt = createdAt,
            IsCompleted = false
        };
        db.TodoItems.Add(todo);
        await db.SaveChangesAsync();
        return todo.Id;
    }

    [Fact]
    public async Task Complete_succeeds_and_sets_completed_at_and_raises_event()
    {
        var user = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var spy = new SpyDomainEventsDispatcher();
        using var provider = BuildProvider(spy, user, now);
        var db = provider.GetRequiredService<ApplicationDbContext>();
        var todoId = await SeedTodo(db, user, now);
        var handler = provider.GetRequiredService<ICommandHandler<CompleteTodoCommand>>();

        var result = await handler.Handle(new CompleteTodoCommand(todoId), default);
        result.IsSuccess.ShouldBeTrue(result.Error?.Description);

        var saved = await db.TodoItems.SingleAsync(t => t.Id == todoId);
        saved.IsCompleted.ShouldBeTrue();
        saved.CompletedAt.ShouldBe(now);

        spy.Events.OfType<TodoItemCompletedDomainEvent>().Any(e => e.TodoItemId == todoId).ShouldBeTrue();
    }

    [Fact]
    public async Task Complete_fails_when_todo_not_found_or_not_owned()
    {
        var user = Guid.NewGuid();
        using var provider = BuildProvider(currentUserId: user);
        var handler = provider.GetRequiredService<ICommandHandler<CompleteTodoCommand>>();

        var result = await handler.Handle(new CompleteTodoCommand(Guid.NewGuid()), default);
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("TodoItems.NotFound");
    }

    [Fact]
    public async Task Complete_fails_when_already_completed()
    {
        var user = Guid.NewGuid();
        var now = DateTime.UtcNow;
        using var provider = BuildProvider(currentUserId: user, now: now);
        var db = provider.GetRequiredService<ApplicationDbContext>();
        var todoId = await SeedTodo(db, user, now);

        // mark completed manually
        var todo = await db.TodoItems.SingleAsync(t => t.Id == todoId);
        todo.IsCompleted = true;
        todo.CompletedAt = now;
        await db.SaveChangesAsync();

        var handler = provider.GetRequiredService<ICommandHandler<CompleteTodoCommand>>();
        var result = await handler.Handle(new CompleteTodoCommand(todoId), default);
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("TodoItems.AlreadyCompleted");
    }

    [Fact]
    public async Task Validator_rejects_empty_id()
    {
        var user = Guid.NewGuid();
        using var provider = BuildProvider(currentUserId: user);
        var handler = provider.GetRequiredService<ICommandHandler<CompleteTodoCommand>>();

        var result = await handler.Handle(new CompleteTodoCommand(Guid.Empty), default);
        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.Validation);
    }
}

