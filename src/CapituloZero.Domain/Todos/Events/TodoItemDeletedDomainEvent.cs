using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Todos.Events;

public sealed record TodoItemDeletedDomainEvent(Guid TodoItemId) : IDomainEvent;
