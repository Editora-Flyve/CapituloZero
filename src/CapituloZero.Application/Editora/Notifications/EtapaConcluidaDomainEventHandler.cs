using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Notifications;
using CapituloZero.Domain.Editora.Events;
using CapituloZero.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Application.Editora.Notifications;

internal sealed class EtapaConcluidaDomainEventHandler(
    IApplicationDbContext context,
    IEmailSender emailSender) : IDomainEventHandler<EtapaConcluidaDomainEvent>
{
    public async Task Handle(EtapaConcluidaDomainEvent notification, CancellationToken cancellationToken)
    {
        var etapa = await context.Etapas.AsNoTracking().SingleOrDefaultAsync(e => e.Id == notification.EtapaId, cancellationToken).ConfigureAwait(false);
        if (etapa is null)
        {
            return;
        }

        string subject = $"Etapa concluída";
        string body = $"A etapa {etapa.Nome} foi concluída.";

        await emailSender.SendAsync("ops@editora.local", subject, body, cancellationToken).ConfigureAwait(false);
    }
}
