using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Todos.GetById;

public sealed record GetTodoByIdQuery(Guid TodoItemId) : IQuery<TodoResponse>;
