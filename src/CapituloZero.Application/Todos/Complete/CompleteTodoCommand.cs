using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Todos.Complete;

public sealed record CompleteTodoCommand(Guid TodoItemId) : ICommand;
