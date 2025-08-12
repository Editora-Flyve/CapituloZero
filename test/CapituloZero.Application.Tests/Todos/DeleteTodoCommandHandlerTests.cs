using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Todos.Delete;
using CapituloZero.Domain.Todos.Entities;
using CapituloZero.Domain.Users.Entities;
using CapituloZero.Infrastructure.Database;
using CapituloZero.Infrastructure.DomainEvents;
using CapituloZero.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Application.Tests.Todos;

public class DeleteTodoCommandHandlerTests
{
    private sealed class FakeUserContext(Guid userId) : IUserContext
    {
        public Guid UserId { get; } = userId;
        public string? ActiveType => null;
    }

    private sealed class NoOpDispatcher : IDomainEventsDispatcher
    {
        public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private static ApplicationDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options, new NoOpDispatcher());
    }

    [Fact]
    public async Task ReturnsNotFoundWhenTodoNotOwned()
    {
        await using var db = CreateInMemoryDb();
        var user = new User { Email = "a@b.com", FirstName = "a", LastName = "b", PasswordHash = "h" };
        var other = new User { Email = "c@d.com", FirstName = "c", LastName = "d", PasswordHash = "h" };
        db.Users.AddRange(user, other);
        var todo = new TodoItem { UserId = other.Id, Description = "x", CreatedAt = DateTime.UtcNow };
        db.TodoItems.Add(todo);
        await db.SaveChangesAsync();

        var handler = new DeleteTodoCommandHandler(db, new FakeUserContext(user.Id));
        var result = await handler.Handle(new DeleteTodoCommand(todo.Id), default);

    Assert.True(result.IsFailure);
    Assert.Equal("TodoItems.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task DeletesTodoWhenOwnerMatches()
    {
        await using var db = CreateInMemoryDb();
        var user = new User { Email = "a@b.com", FirstName = "a", LastName = "b", PasswordHash = "h" };
        db.Users.Add(user);
        var todo = new TodoItem { UserId = user.Id, Description = "x", CreatedAt = DateTime.UtcNow };
        db.TodoItems.Add(todo);
        await db.SaveChangesAsync();

        var handler = new DeleteTodoCommandHandler(db, new FakeUserContext(user.Id));
        var result = await handler.Handle(new DeleteTodoCommand(todo.Id), default);

        Assert.True(result.IsSuccess);
        Assert.Empty(db.TodoItems);
    }
}
