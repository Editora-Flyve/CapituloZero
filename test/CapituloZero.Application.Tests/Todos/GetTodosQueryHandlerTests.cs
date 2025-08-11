using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Todos.List;
using CapituloZero.Domain.Todos.Entities;
using CapituloZero.Domain.Users.Entities;
using CapituloZero.Infrastructure.Database;
using CapituloZero.Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Application.Tests.Todos;

public class GetTodosQueryHandlerTests
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
    public async Task Returns_Unauthorized_When_Context_Differs()
    {
        await using var db = CreateInMemoryDb();
        var handler = new GetTodosQueryHandler(db, new FakeUserContext(Guid.NewGuid()));
        var result = await handler.Handle(new GetTodosQuery(Guid.NewGuid()), default);
        Assert.True(result.IsFailure);
        Assert.Equal("Users.Unauthorized", result.Error.Code);
    }

    [Fact]
    public async Task Returns_Todos_For_User()
    {
        await using var db = CreateInMemoryDb();
        var user = new User { Email = "a@b.com", FirstName = "a", LastName = "b", PasswordHash = "h" };
        db.Users.Add(user);
        db.TodoItems.AddRange(
            new TodoItem { UserId = user.Id, Description = "x", CreatedAt = DateTime.UtcNow },
            new TodoItem { UserId = user.Id, Description = "y", CreatedAt = DateTime.UtcNow },
            new TodoItem { UserId = Guid.NewGuid(), Description = "z", CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var handler = new GetTodosQueryHandler(db, new FakeUserContext(user.Id));
        var result = await handler.Handle(new GetTodosQuery(user.Id), default);
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.DoesNotContain(result.Value, t => t.Description == "z");
    }
}
