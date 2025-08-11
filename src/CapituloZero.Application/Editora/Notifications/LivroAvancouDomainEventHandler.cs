using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Notifications;
using CapituloZero.Domain.Editora.Events;
using CapituloZero.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Application.Editora.Notifications;

internal sealed class LivroAvancouDomainEventHandler(
    IApplicationDbContext context,
    IEmailSender emailSender) : IDomainEventHandler<LivroAvancouDomainEvent>
{
    public async Task Handle(LivroAvancouDomainEvent notification, CancellationToken cancellationToken)
    {
        var livro = await context.Livros.AsNoTracking().SingleOrDefaultAsync(l => l.Id == notification.LivroId, cancellationToken).ConfigureAwait(false);
        if (livro is null)
        {
            return;
        }

        string subject = "Livro avançou";
        string body = $"O livro {livro.Titulo} avançou para a próxima etapa.";

        await emailSender.SendAsync("ops@editora.local", subject, body, cancellationToken).ConfigureAwait(false);
    }
}
