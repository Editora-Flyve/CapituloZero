using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Users;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
