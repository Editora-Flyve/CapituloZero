using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Domain.Todos;
using CapituloZero.Domain.Users;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Todos.Create;

internal sealed class CreateTodoCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext)
    : ICommandHandler<CreateTodoCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
    if (userContext.UserId != (Guid)command.UserId)
        {
            return Result.Failure<Guid>(UserErrors.Unauthorized());
        }

        var todoItem = new TodoItem
        {
            UserId = command.UserId,
            Description = command.Description,
            Priority = command.Priority,
            DueDate = command.DueDate,
            Labels = command.Labels,
            IsCompleted = false,
            CreatedAt = dateTimeProvider.UtcNow
        };

        // Adiciona primeiro para garantir que o EF gere o Id do agregado
        context.TodoItems.Add(todoItem);

        // Agora levanta o evento com o Id correto
        todoItem.Raise(new TodoItemCreatedDomainEvent(todoItem.Id));

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return todoItem.Id;
    }
}
