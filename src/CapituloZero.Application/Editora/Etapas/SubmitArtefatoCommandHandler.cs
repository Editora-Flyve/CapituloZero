using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Domain.Editora.Entities;
using CapituloZero.Domain.Editora.Events;
using Microsoft.EntityFrameworkCore;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Editora.Etapas;

internal sealed class SubmitArtefatoCommandHandler(IApplicationDbContext context)
    : ICommandHandler<SubmitArtefatoCommand>
{
    public async Task<Result> Handle(SubmitArtefatoCommand command, CancellationToken cancellationToken)
    {
        Etapa? etapa = await context.Etapas
            .SingleOrDefaultAsync(e => e.Id == command.EtapaId, cancellationToken)
            .ConfigureAwait(false);

        if (etapa is null)
        {
            return Result.Failure(Error.NotFound("Etapa.NotFound", "Stage not found"));
        }

        var artefato = new Artefato
        {
            EtapaId = etapa.Id,
            FileUri = command.FileUri,
            FileName = command.FileName,
            ContentType = command.ContentType,
            SizeBytes = command.SizeBytes,
            UploadedByUserId = command.UploadedByUserId
        };

        etapa.SubmitArtefato(artefato);

        etapa.Raise(new ArtefatoSubmetidoDomainEvent(etapa.Id, artefato.FileUri));

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
