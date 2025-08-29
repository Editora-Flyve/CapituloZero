using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Todos.Create;
using CapituloZero.Domain.Todos;
using CapituloZero.Domain.Users;
using CapituloZero.Infrastructure.Database;
using CapituloZero.ApplicationTests.Helpers;
using CapituloZero.SharedKernel;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace CapituloZero.ApplicationTests.Todos;

public class CreateTodoTests
{
    private static readonly string[] LabelsPool = ["home", "work", "shopping", "study"];
    private static Faker _faker = new("pt_BR");

    [Fact]
    public async Task CreateSucceedsAndRaisesDomainEvent()
    {
        var currentUser = Guid.NewGuid();
        var now = new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var spy = new SpyDomainEventsDispatcher();
        using var provider = TestServiceProvider.Build(spy, currentUser, now, dbName: $"todos-create-{Guid.NewGuid()}");
        var db = provider.GetRequiredService<ApplicationDbContext>();
        var handler = provider.GetRequiredService<ICommandHandler<CreateTodoCommand, Guid>>();

        var cmd = new CreateTodoCommand
        {
            UserId = currentUser,
            Description = _faker.Lorem.Sentence(3),
            Priority = _faker.PickRandom<Priority>(),
            DueDate = now.AddDays(1),
            Labels = _faker.Random.ListItems(LabelsPool, _faker.Random.Int(1, 3)).ToList()
        };

        var result = await handler.Handle(cmd, default);
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);

        var saved = await db.TodoItems.SingleAsync(t => t.Id == result.Value);
        saved.UserId.ShouldBe((UserId)currentUser);
        saved.Description.ShouldBe(cmd.Description);
        saved.CreatedAt.ShouldBe(now);
        saved.IsCompleted.ShouldBeFalse();

        spy.Events.OfType<TodoItemCreatedDomainEvent>().Any(e => e.TodoItemId == saved.Id).ShouldBeTrue();
    }

    [Fact]
    public async Task CreateFailsWhenUserContextDiffers()
    {
        var currentUser = Guid.NewGuid();
        using var provider = TestServiceProvider.Build(currentUserId: currentUser, dbName: $"todos-create-{Guid.NewGuid()}");
        var handler = provider.GetRequiredService<ICommandHandler<CreateTodoCommand, Guid>>();

        var cmd = new CreateTodoCommand
        {
            UserId = Guid.NewGuid(),
            Description = "X",
            Priority = Priority.Low
        };

        var result = await handler.Handle(cmd, default);
        result.IsFailure.ShouldBeTrue();
        result.ErrorInternal.Code.ShouldBe(UserErrors.Unauthorized().Code);
    }

    [Fact]
    public async Task CreateFailsValidationWhenDescriptionEmpty()
    {
        var currentUser = Guid.NewGuid();
        using var provider = TestServiceProvider.Build(currentUserId: currentUser, dbName: $"todos-create-{Guid.NewGuid()}");
        var handler = provider.GetRequiredService<ICommandHandler<CreateTodoCommand, Guid>>();

        var cmd = new CreateTodoCommand
        {
            UserId = currentUser,
            Description = string.Empty,
            Priority = Priority.Low
        };

        var result = await handler.Handle(cmd, default);
        result.IsFailure.ShouldBeTrue();
        result.ErrorInternal.Type.ShouldBe(ErrorType.Validation);
    }
}