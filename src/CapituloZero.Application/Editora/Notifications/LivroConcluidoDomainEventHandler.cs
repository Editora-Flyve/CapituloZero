using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Notifications;
using CapituloZero.Domain.Editora.Events;
using CapituloZero.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Application.Editora.Notifications;

internal sealed class LivroConcluidoDomainEventHandler(
    IApplicationDbContext context,
    IEmailSender emailSender) : IDomainEventHandler<LivroConcluidoDomainEvent>
{
    public async Task Handle(LivroConcluidoDomainEvent notification, CancellationToken cancellationToken)
    {
        var livro = await context.Livros.AsNoTracking().SingleOrDefaultAsync(l => l.Id == notification.LivroId, cancellationToken).ConfigureAwait(false);
        if (livro is null)
        {
            return;
        }

        string subject = "Livro concluído";
        string body = $"O livro {livro.Titulo} foi concluído.";

        await emailSender.SendAsync("ops@editora.local", subject, body, cancellationToken).ConfigureAwait(false);
    }
}
