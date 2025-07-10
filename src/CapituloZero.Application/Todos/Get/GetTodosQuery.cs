using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Todos.Get;

public sealed record GetTodosQuery(Guid UserId) : IQuery<List<TodoResponse>>;
