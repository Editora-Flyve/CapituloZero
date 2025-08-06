using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Todos.Events;

public sealed record TodoItemCompletedDomainEvent(Guid TodoItemId) : IDomainEvent;
