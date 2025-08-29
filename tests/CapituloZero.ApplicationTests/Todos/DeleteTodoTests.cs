using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Todos.Delete;
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

public class DeleteTodoTests
{
    private static async Task<Guid> SeedTodo(ApplicationDbContext db, Guid userId)
    {
        var todo = new TodoItem
        {
            UserId = userId,
            Description = "Task to delete",
            Priority = Priority.Medium,
            CreatedAt = DateTime.UtcNow,
            IsCompleted = false
        };
        db.TodoItems.Add(todo);
        await db.SaveChangesAsync();
        return todo.Id;
    }

    [Fact]
    public async Task DeleteRemovesEntityAndRaisesEvent()
    {
        var user = Guid.NewGuid();
        var dispatcher = Substitute.For<IDomainEventsDispatcher>();
        using var provider = TestServiceProvider.Build(dispatcher, currentUserId: user, dbName: $"todos-delete-{Guid.NewGuid()}");
        var db = provider.GetRequiredService<ApplicationDbContext>();
        var todoId = await SeedTodo(db, user);

        dispatcher.ClearReceivedCalls();

        var handler = provider.GetRequiredService<ICommandHandler<DeleteTodoCommand>>();
        var result = await handler.Handle(new DeleteTodoCommand(todoId), default);
        result.IsSuccess.ShouldBeTrue();

        (await db.TodoItems.AnyAsync(t => t.Id == todoId)).ShouldBeFalse();

        dispatcher.Received(1).DispatchAsync(
            Arg.Any<IEnumerable<IDomainEvent>>(),
            Arg.Any<CancellationToken>());

        // Captura o argumento real passado para DispatchAsync
        var dispatchedEvents = dispatcher.ReceivedCalls()
            .First(call => call.GetMethodInfo().Name == nameof(IDomainEventsDispatcher.DispatchAsync))
            .GetArguments()[0] as IEnumerable<IDomainEvent>;

        dispatchedEvents.ShouldNotBeNull();
        dispatchedEvents.Any().ShouldBeTrue();
        dispatchedEvents.Any(e =>
            e is TodoItemDeletedDomainEvent && ((TodoItemDeletedDomainEvent)e).TodoItemId == todoId
        ).ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteFailsWhenNotFoundOrNotOwned()
    {
        var user = Guid.NewGuid();
        var dispatcher = Substitute.For<IDomainEventsDispatcher>();
        using var provider = TestServiceProvider.Build(dispatcher, currentUserId: user, dbName: $"todos-delete-{Guid.NewGuid()}");
        var handler = provider.GetRequiredService<ICommandHandler<DeleteTodoCommand>>();

        var result = await handler.Handle(new DeleteTodoCommand(Guid.NewGuid()), default);
        result.IsFailure.ShouldBeTrue();
        result.ErrorInternal.Code.ShouldBe("TodoItems.NotFound");

        dispatcher.DidNotReceive().DispatchAsync(Arg.Any<IEnumerable<IDomainEvent>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ValidatorRejectsEmptyId()
    {
        var user = Guid.NewGuid();
        var dispatcher = Substitute.For<IDomainEventsDispatcher>();
        using var provider = TestServiceProvider.Build(dispatcher, currentUserId: user, dbName: $"todos-delete-{Guid.NewGuid()}");
        var handler = provider.GetRequiredService<ICommandHandler<DeleteTodoCommand>>();

        var result = await handler.Handle(new DeleteTodoCommand(Guid.Empty), default);
        result.IsFailure.ShouldBeTrue();
        result.ErrorInternal.Type.ShouldBe(ErrorType.Validation);

        dispatcher.DidNotReceive().DispatchAsync(Arg.Any<IEnumerable<IDomainEvent>>(), Arg.Any<CancellationToken>());
    }
}
