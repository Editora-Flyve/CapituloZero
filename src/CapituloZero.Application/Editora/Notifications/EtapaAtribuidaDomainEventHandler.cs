using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Notifications;
using CapituloZero.Domain.Editora.Events;
using CapituloZero.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Application.Editora.Notifications;

internal sealed class EtapaAtribuidaDomainEventHandler(
    IApplicationDbContext context,
    IEmailSender emailSender) : IDomainEventHandler<EtapaAtribuidaDomainEvent>
{
    public async Task Handle(EtapaAtribuidaDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var terceiro = await context.Terceiros.SingleOrDefaultAsync(t => t.Id == domainEvent.TerceiroId, cancellationToken).ConfigureAwait(false);
        if (terceiro is null)
        {
            return;
        }

        string subject = "Etapa atribuída";
        string body = $"Você foi atribuído à etapa {domainEvent.EtapaId} do livro {domainEvent.LivroId}.";
        await emailSender.SendAsync(terceiro.Email, subject, body, cancellationToken).ConfigureAwait(false);
    }
}
