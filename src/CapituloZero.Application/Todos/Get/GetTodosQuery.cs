using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Todos.List;

public sealed record GetTodosQuery(Guid UserId) : IQuery<List<TodoResponse>>;
