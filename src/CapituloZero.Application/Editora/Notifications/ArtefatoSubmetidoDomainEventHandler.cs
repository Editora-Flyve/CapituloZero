using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Notifications;
using CapituloZero.Domain.Editora.Events;
using CapituloZero.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Application.Editora.Notifications;

internal sealed class ArtefatoSubmetidoDomainEventHandler(
    IApplicationDbContext context,
    IEmailSender emailSender) : IDomainEventHandler<ArtefatoSubmetidoDomainEvent>
{
    public async Task Handle(ArtefatoSubmetidoDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var etapa = await context.Etapas.SingleOrDefaultAsync(e => e.Id == domainEvent.EtapaId, cancellationToken).ConfigureAwait(false);
        if (etapa is null || etapa.ResponsavelId is null)
        {
            return;
        }

        string subject = "Artefato submetido";
        string body = $"Um novo artefato foi submetido na etapa {domainEvent.EtapaId}: {domainEvent.FileUri}.";

        // Notify ops; in a real impl, resolve subscribers (responsável, editor, autor)
        await emailSender.SendAsync("ops@editora.local", subject, body, cancellationToken).ConfigureAwait(false);
    }
}
