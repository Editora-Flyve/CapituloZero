using CapituloZero.Domain.Users.Events;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Users.Register;

internal sealed class UserRegisteredDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public Task Handle(UserRegisteredDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // Future: enqueue email verification notification.
        return Task.CompletedTask;
    }
}
