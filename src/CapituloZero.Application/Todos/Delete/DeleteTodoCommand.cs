using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Todos.Delete;

public sealed record DeleteTodoCommand(Guid TodoItemId) : ICommand;
