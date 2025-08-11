using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Todos.Create;
using CapituloZero.Domain.Todos.Entities;
using CapituloZero.Domain.Todos.Enums;
using CapituloZero.Domain.Users.Entities;
using CapituloZero.Infrastructure.Database;
using CapituloZero.Infrastructure.DomainEvents;
using CapituloZero.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Application.Tests.Todos;

file sealed class FakeUserContext(Guid userId) : IUserContext
{
    public Guid UserId { get; } = userId;
    public string? ActiveType => null;
}

file sealed class NoOpDispatcher : IDomainEventsDispatcher
{
    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

public class CreateTodoCommandHandlerTests
{
    private static ApplicationDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options, new NoOpDispatcher());
    }

    [Fact]
    public async Task Returns_Unauthorized_When_UserContext_Differs()
    {
        await using var db = CreateInMemoryDb();
        var handler = new CreateTodoCommandHandler(db, new SystemDateTimeProvider(), new FakeUserContext(Guid.NewGuid()));
        var cmd = new CreateTodoCommand(Guid.NewGuid(), "desc", Priority.Low, DateTime.UtcNow.AddDays(1), []);

        var result = await handler.Handle(cmd, default);

        Assert.True(result.IsFailure);
        Assert.Equal("Users.Unauthorized", result.Error.Code);
    }

    [Fact]
    public async Task Creates_Todo_When_User_Exists_And_Context_Matches()
    {
        await using var db = CreateInMemoryDb();
        var user = new User { Email = "a@b.com", FirstName = "a", LastName = "b", PasswordHash = "h" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var handler = new CreateTodoCommandHandler(db, new SystemDateTimeProvider(), new FakeUserContext(user.Id));
        var cmd = new CreateTodoCommand(user.Id, "desc", Priority.High, DateTime.UtcNow.AddDays(1), new []{"work"});

        var result = await handler.Handle(cmd, default);

        Assert.True(result.IsSuccess);
        var saved = await db.TodoItems.SingleAsync(t => t.Id == result.Value);
        Assert.Equal("desc", saved.Description);
        Assert.Contains("work", saved.Labels);
        Assert.False(saved.IsCompleted);
    }
}

file sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
