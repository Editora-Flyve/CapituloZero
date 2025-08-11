using CapituloZero.Application.Abstractions.Notifications;
using CapituloZero.Domain.Editora.Events;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Editora.Notifications;

internal sealed class LivroCriadoDomainEventHandler(IEmailSender emailSender) : IDomainEventHandler<LivroCriadoDomainEvent>
{
    public Task Handle(LivroCriadoDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        return emailSender.SendAsync("ops@editora.local", "Book created", $"Livro {domainEvent.LivroId} created.", cancellationToken);
    }
}
