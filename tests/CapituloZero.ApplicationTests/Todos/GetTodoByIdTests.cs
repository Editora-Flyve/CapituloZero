using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Todos.GetById;
using CapituloZero.Domain.Todos;
using CapituloZero.Infrastructure.Database;
using CapituloZero.ApplicationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace CapituloZero.ApplicationTests.Todos;

public class GetTodoByIdTests
{
    private static async Task<Guid> SeedTodo(ApplicationDbContext db, Guid userId)
    {
        var todo = new TodoItem
        {
            UserId = userId,
            Description = "ById Task",
            Priority = Priority.Low,
            CreatedAt = DateTime.UtcNow,
            IsCompleted = false
        };
        db.TodoItems.Add(todo);
        await db.SaveChangesAsync().ConfigureAwait(false);
        return todo.Id;
    }

    [Fact]
    public async Task GetByIdReturnsOwnedTodo()
    {
        var user = Guid.NewGuid();
        await using var provider = TestServiceProvider.Build(currentUserId: user, dbName: $"todos-getbyid-{Guid.NewGuid()}");
        var db = provider.GetRequiredService<ApplicationDbContext>();
        var todoId = await SeedTodo(db, user);
        var handler = provider.GetRequiredService<IQueryHandler<GetTodoByIdQuery, TodoResponse>>();

        var result = await handler.Handle(new GetTodoByIdQuery(todoId), default);
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(todoId);
        result.Value.UserId.ShouldBe(user);
    }

    [Fact]
    public async Task GetByIdFailsWhenNotFoundOrNotOwned()
    {
        var user = Guid.NewGuid();
        await using var provider = TestServiceProvider.Build(currentUserId: user, dbName: $"todos-getbyid-{Guid.NewGuid()}");
        var handler = provider.GetRequiredService<IQueryHandler<GetTodoByIdQuery, TodoResponse>>();

        var result = await handler.Handle(new GetTodoByIdQuery(Guid.NewGuid()), default);
        result.IsFailure.ShouldBeTrue();
        result.ErrorInternal.Code.ShouldBe("TodoItems.NotFound");
    }
}

