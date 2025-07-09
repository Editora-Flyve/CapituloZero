using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Todos;

public sealed record TodoItemCompletedDomainEvent(Guid TodoItemId) : IDomainEvent;
