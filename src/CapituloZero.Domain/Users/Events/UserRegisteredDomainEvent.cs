using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Users.Events;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
