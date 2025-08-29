using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Todos.Get;
using CapituloZero.Domain.Todos;
using CapituloZero.Infrastructure.Database;
using CapituloZero.ApplicationTests.Helpers;
using CapituloZero.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace CapituloZero.ApplicationTests.Todos;

public class GetTodosTests
{
    private static async Task SeedTodos(ApplicationDbContext db, Guid userId, int count)
    {
        for (int i = 0; i < count; i++)
        {
            db.TodoItems.Add(new TodoItem
            {
                UserId = userId,
                Description = $"Task {i}",
                Priority = Priority.Low,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false
            });
        }
        // outros de outro usuário
        db.TodoItems.Add(new TodoItem
        {
            UserId = Guid.NewGuid(),
            Description = "Other user task",
            Priority = Priority.Medium,
            CreatedAt = DateTime.UtcNow,
            IsCompleted = false
        });
        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task GetTodosReturnsOnlyOwned()
    {
        var user = Guid.NewGuid();
        await using var provider = TestServiceProvider.Build(currentUserId: user, dbName: $"todos-get-{Guid.NewGuid()}");
        var db = provider.GetRequiredService<ApplicationDbContext>();
        await SeedTodos(db, user, 3);
        var handler = provider.GetRequiredService<IQueryHandler<GetTodosQuery, List<TodoResponse>>>();

        var result = await handler.Handle(new GetTodosQuery(user), default);
        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(3);
        result.Value.All(t => t.UserId == user).ShouldBeTrue();
    }

    [Fact]
    public async Task GetTodosFailsWhenUserDiffersFromContext()
    {
        var user = Guid.NewGuid();
        await using var provider = TestServiceProvider.Build(currentUserId: user, dbName: $"todos-get-{Guid.NewGuid()}");
        var handler = provider.GetRequiredService<IQueryHandler<GetTodosQuery, List<TodoResponse>>>();

        var result = await handler.Handle(new GetTodosQuery(Guid.NewGuid()), default);
        result.IsFailure.ShouldBeTrue();
        result.ErrorInternal.Code.ShouldBe("Users.Unauthorized");
        result.ErrorInternal.Type.ShouldBe(ErrorType.Problem);
    }
}

