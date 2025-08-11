using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Domain.Editora.Entities;
using Microsoft.EntityFrameworkCore;
using CapituloZero.SharedKernel;
using CapituloZero.Domain.Editora.Events;

namespace CapituloZero.Application.Editora.Etapas;

internal sealed class AssignTerceiroToEtapaCommandHandler(IApplicationDbContext context)
    : ICommandHandler<AssignTerceiroToEtapaCommand>
{
    public async Task<Result> Handle(AssignTerceiroToEtapaCommand command, CancellationToken cancellationToken)
    {
        Etapa? etapa = await context.Etapas
            .Include(e => e.Funcao)
            .SingleOrDefaultAsync(e => e.Id == command.EtapaId, cancellationToken)
            .ConfigureAwait(false);

        if (etapa is null)
        {
            return Result.Failure(Error.NotFound("Etapa.NotFound", "Stage not found"));
        }

        Terceiro? terceiro = await context.Terceiros
            .Include(t => t.Funcao)
            .SingleOrDefaultAsync(t => t.Id == command.TerceiroId, cancellationToken)
            .ConfigureAwait(false);

        if (terceiro is null)
        {
            return Result.Failure(Error.NotFound("Terceiro.NotFound", "Third-party not found"));
        }

        if (terceiro.Funcao.Id != etapa.Funcao.Id)
        {
            return Result.Failure(Error.Problem("Funcao.Mismatch", "Third-party role does not match stage role"));
        }

        etapa.AssignResponsavel(terceiro.Id);

        // Raise event for notification pipeline
        var livroId = etapa.LivroId;
        var etapaId = etapa.Id;
        etapa.Raise(new EtapaAtribuidaDomainEvent(livroId, etapaId, terceiro.Id));

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
