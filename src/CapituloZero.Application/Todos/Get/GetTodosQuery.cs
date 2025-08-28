using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Todos.Get;

public sealed record GetTodosQuery(UserId UserId) : IQuery<List<TodoResponse>>;
