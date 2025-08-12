using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Users.Login;
using CapituloZero.Domain.Users;
using CapituloZero.Domain.Users.Entities;
using CapituloZero.Infrastructure.Database;
using CapituloZero.Infrastructure.DomainEvents;
using CapituloZero.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Application.Tests.Users;

public class LoginUserCommandHandlerTests
{
    private sealed class NoOpDispatcher : IDomainEventsDispatcher
    {
        public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => password;
        public bool Verify(string password, string hash) => password == hash;
    }

    private sealed class FakeTokenProvider : ITokenProvider
    {
        public string Create(User user) => $"token:{user.Id}:{user.ActiveType}";
    }

    private static ApplicationDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options, new NoOpDispatcher());
    }

    [Fact]
    public async Task ReturnsNotFoundWhenUserDoesNotExist()
    {
        await using var db = CreateInMemoryDb();
        var handler = new LoginUserCommandHandler(db, new FakePasswordHasher(), new FakeTokenProvider());
        var result = await handler.Handle(new LoginUserCommand("x@y.com", "pw", null), default);
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.NotFoundByEmail, result.Error);
    }

    [Fact]
    public async Task ReturnsSelectTypesWhenMultipleAndNoDesiredType()
    {
        await using var db = CreateInMemoryDb();
        var user = new User { Email = "a@b.com", FirstName = "a", LastName = "b", PasswordHash = "pw", Types = UserType.Cliente | UserType.Terceiro };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var handler = new LoginUserCommandHandler(db, new FakePasswordHasher(), new FakeTokenProvider());
        var result = await handler.Handle(new LoginUserCommand(user.Email, "pw", null), default);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.RequiresSelection);
        Assert.Null(result.Value.Token);
        Assert.Equal(2, result.Value.AvailableTypes!.Count);
    }

    [Fact]
    public async Task ReturnsTokenWhenTypeSelected()
    {
        await using var db = CreateInMemoryDb();
        var user = new User { Email = "a@b.com", FirstName = "a", LastName = "b", PasswordHash = "pw", Types = UserType.Cliente | UserType.Terceiro };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var handler = new LoginUserCommandHandler(db, new FakePasswordHasher(), new FakeTokenProvider());
        var result = await handler.Handle(new LoginUserCommand(user.Email, "pw", UserType.Cliente), default);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value.RequiresSelection);
        Assert.NotNull(result.Value.Token);
        Assert.Equal(UserType.Cliente, result.Value.ActiveType);
    }
}
