using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Todos;

public sealed record TodoItemDeletedDomainEvent(Guid TodoItemId) : IDomainEvent;
