using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Todos.Events;

public sealed record TodoItemCreatedDomainEvent(Guid TodoItemId) : IDomainEvent;
