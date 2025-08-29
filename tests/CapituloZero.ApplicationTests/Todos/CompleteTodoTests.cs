using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Todos.Complete;
using CapituloZero.Domain.Todos;
using CapituloZero.Infrastructure.Database;
using CapituloZero.Infrastructure.DomainEvents;
using CapituloZero.SharedKernel;
using CapituloZero.ApplicationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Xunit;

namespace CapituloZero.ApplicationTests.Todos;

public class CompleteTodoTests
{
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
    public async Task CompleteSucceedsAndSetsCompletedAtAndRaisesEvent()
    {
        var user = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dispatcher = Substitute.For<IDomainEventsDispatcher>();
        using var provider = TestServiceProvider.Build(dispatcher, user, now, dbName: $"todos-complete-{Guid.NewGuid()}");
        var db = provider.GetRequiredService<ApplicationDbContext>();
        var todoId = await SeedTodo(db, user, now);
        var handler = provider.GetRequiredService<ICommandHandler<CompleteTodoCommand>>();

        var result = await handler.Handle(new CompleteTodoCommand(todoId), default);
        result.IsSuccess.ShouldBeTrue();

        var saved = await db.TodoItems.SingleAsync(t => t.Id == todoId);
        saved.IsCompleted.ShouldBeTrue();
        saved.CompletedAt.ShouldBe(now);

        await dispatcher.Received(1).DispatchAsync(
            Arg.Is<IEnumerable<IDomainEvent>>(events =>
                events.OfType<TodoItemCompletedDomainEvent>().Any(e => e.TodoItemId == todoId)
            ),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CompleteFailsWhenTodoNotFoundOrNotOwned()
    {
        var user = Guid.NewGuid();
        var dispatcher = Substitute.For<IDomainEventsDispatcher>();
        using var provider = TestServiceProvider.Build(dispatcher, currentUserId: user, dbName: $"todos-complete-{Guid.NewGuid()}");
        var handler = provider.GetRequiredService<ICommandHandler<CompleteTodoCommand>>();

        var result = await handler.Handle(new CompleteTodoCommand(Guid.NewGuid()), default);
        result.IsFailure.ShouldBeTrue();
        result.ErrorInternal.Code.ShouldBe("TodoItems.NotFound");

        await dispatcher.DidNotReceive().DispatchAsync(Arg.Any<IEnumerable<IDomainEvent>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CompleteFailsWhenAlreadyCompleted()
    {
        var user = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dispatcher = Substitute.For<IDomainEventsDispatcher>();
        using var provider = TestServiceProvider.Build(dispatcher, currentUserId: user, now: now, dbName: $"todos-complete-{Guid.NewGuid()}");
        var db = provider.GetRequiredService<ApplicationDbContext>();
        var todoId = await SeedTodo(db, user, now);

        // marcar manualmente como completo
        var todo = await db.TodoItems.SingleAsync(t => t.Id == todoId);
        todo.IsCompleted = true;
        todo.CompletedAt = now;
        await db.SaveChangesAsync();

        dispatcher.ClearReceivedCalls();

        var handler = provider.GetRequiredService<ICommandHandler<CompleteTodoCommand>>();
        var result = await handler.Handle(new CompleteTodoCommand(todoId), default);
        result.IsFailure.ShouldBeTrue();
        result.ErrorInternal.Code.ShouldBe("TodoItems.AlreadyCompleted");

        await dispatcher.DidNotReceive().DispatchAsync(Arg.Any<IEnumerable<IDomainEvent>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ValidatorRejectsEmptyId()
    {
        var user = Guid.NewGuid();
        var dispatcher = Substitute.For<IDomainEventsDispatcher>();
        using var provider = TestServiceProvider.Build(dispatcher, currentUserId: user, dbName: $"todos-complete-{Guid.NewGuid()}");
        var handler = provider.GetRequiredService<ICommandHandler<CompleteTodoCommand>>();

        var result = await handler.Handle(new CompleteTodoCommand(Guid.Empty), default);
        result.IsFailure.ShouldBeTrue();
        result.ErrorInternal.Type.ShouldBe(ErrorType.Validation);

        await dispatcher.DidNotReceive().DispatchAsync(Arg.Any<IEnumerable<IDomainEvent>>(), Arg.Any<CancellationToken>());
    }
}