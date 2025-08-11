using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Todos.Complete;
using CapituloZero.Domain.Todos.Entities;
using CapituloZero.Domain.Users.Entities;
using CapituloZero.Infrastructure.Database;
using CapituloZero.Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Application.Tests.Todos;

public class CompleteTodoCommandHandlerTests
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

    private sealed class FakeClock(DateTime now) : IDateTimeProvider
    {
        public DateTime UtcNow { get; } = now;
    }

    private static ApplicationDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options, new NoOpDispatcher());
    }

    [Fact]
    public async Task Returns_NotFound_When_Todo_Missing()
    {
        await using var db = CreateInMemoryDb();
        var handler = new CompleteTodoCommandHandler(db, new FakeClock(DateTime.UtcNow), new FakeUserContext(Guid.NewGuid()));
        var result = await handler.Handle(new CompleteTodoCommand(Guid.NewGuid()), default);
        Assert.True(result.IsFailure);
        Assert.Equal("TodoItems.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Completes_Todo_When_Valid()
    {
        await using var db = CreateInMemoryDb();
        var user = new User { Email = "a@b.com", FirstName = "a", LastName = "b", PasswordHash = "h" };
        db.Users.Add(user);
        var todo = new TodoItem { UserId = user.Id, Description = "x", CreatedAt = DateTime.UtcNow };
        db.TodoItems.Add(todo);
        await db.SaveChangesAsync();

        var now = DateTime.UtcNow;
        var handler = new CompleteTodoCommandHandler(db, new FakeClock(now), new FakeUserContext(user.Id));
        var result = await handler.Handle(new CompleteTodoCommand(todo.Id), default);

        Assert.True(result.IsSuccess);
        var saved = await db.TodoItems.SingleAsync();
        Assert.True(saved.IsCompleted);
        Assert.Equal(now, saved.CompletedAt);
    }
}
