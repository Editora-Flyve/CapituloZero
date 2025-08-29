using CapituloZero.Application;
using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Users.GetByEmail;
using CapituloZero.Application.Users.GetById;
using CapituloZero.Application.Users.Register;
using CapituloZero.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace CapituloZero.ApplicationTests.Users;

public class UserHandlersTests
{
    private sealed class FakeIdentityService : IIdentityService
    {
        public Guid CurrentUserId { get; set; }
        public Result<Guid> RegisterResult { get; set; } = Result.Success(Guid.NewGuid());
        public Result<Application.Users.Login.LoginResponse> LoginAsyncResult { get; set; } = Result.Failure<Application.Users.Login.LoginResponse>(ErrorInternal.Problem("Not", "used"));
        public Result<Application.Users.Login.LoginResponse> RefreshAsyncResult { get; set; } = Result.Failure<Application.Users.Login.LoginResponse>(ErrorInternal.Problem("Not", "used"));
        public Result<Application.Users.GetById.UserResponse> GetByIdResult { get; set; } = Result.Success(new Application.Users.GetById.UserResponse { Id = Guid.NewGuid(), Email = "a@b.c", FirstName = "A", LastName = "B" });
        public Result<Application.Users.GetByEmail.UserResponse> GetByEmailResult { get; set; } = Result.Success(new Application.Users.GetByEmail.UserResponse { Id = Guid.NewGuid(), Email = "a@b.c", FirstName = "A", LastName = "B" });
        public Result AddTypesResult { get; set; } = Result.Success();
        public Task<Result<Guid>> RegisterAsync(string email, string firstName, string lastName, string password, CancellationToken ct = default) => Task.FromResult(RegisterResult);
        public Task<Result<Application.Users.Login.LoginResponse>> LoginAsync(string email, string password, CancellationToken ct = default) => Task.FromResult(LoginAsyncResult);
        public Task<Result<Application.Users.Login.LoginResponse>> RefreshAsync(string refreshToken, CancellationToken ct = default) => Task.FromResult(RefreshAsyncResult);
        public Task<Result<Application.Users.GetById.UserResponse>> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default) => Task.FromResult(GetByIdResult);
        public Task<Result<Application.Users.GetByEmail.UserResponse>> GetByEmailAsync(string email, Guid currentUserId, CancellationToken ct = default) => Task.FromResult(GetByEmailResult);
        public Task<Result> AddUserTypesAsync(Guid userId, System.Collections.Generic.IEnumerable<string> tipos, CancellationToken ct = default) => Task.FromResult(AddTypesResult);
        public Task<Result<System.Collections.Generic.IReadOnlyList<Application.Users.Get.UserListItemResponse>>> GetAllAsync(CancellationToken ct = default) => Task.FromResult(Result.Success<System.Collections.Generic.IReadOnlyList<Application.Users.Get.UserListItemResponse>>(Array.Empty<Application.Users.Get.UserListItemResponse>()));
    }

    private static ServiceProvider BuildProvider(FakeIdentityService fake)
    {
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddSingleton<IIdentityService>(fake);
        services.AddLogging();
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task GetUserByIdHandlerReturnsIdentityResult()
    {
        var fake = new FakeIdentityService();
        await using var provider = BuildProvider(fake);
        var handler = provider.GetRequiredService<IQueryHandler<GetUserByIdQuery, Application.Users.GetById.UserResponse>>();
        var res = await handler.Handle(new GetUserByIdQuery(Guid.NewGuid()), default);
        res.IsSuccess.ShouldBeTrue();
        res.Value.Email.ShouldBe("a@b.c");
    }

    [Fact]
    public async Task GetUserByEmailHandlerReturnsIdentityResult()
    {
        var fake = new FakeIdentityService();
        await using var provider = BuildProvider(fake);
        var handler = provider.GetRequiredService<IQueryHandler<GetUserByEmailQuery, Application.Users.GetByEmail.UserResponse>>();
        var res = await handler.Handle(new GetUserByEmailQuery("a@b.c"), default);
        res.IsSuccess.ShouldBeTrue();
        res.Value.FirstName.ShouldBe("A");
    }

    [Fact]
    public async Task RegisterHandlerReturnsIdentityResult()
    {
        var fake = new FakeIdentityService();
        var expectedId = Guid.NewGuid();
        fake.RegisterResult = Result.Success(expectedId);
        await using var provider = BuildProvider(fake);
        var handler = provider.GetRequiredService<ICommandHandler<RegisterUserCommand, Guid>>();
        var res = await handler.Handle(new RegisterUserCommand("x@y.z", "X", "Y", "pwd"), default);
        res.IsSuccess.ShouldBeTrue();
        res.Value.ShouldBe(expectedId);
    }
}

