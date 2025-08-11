using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Domain.Editora.Entities;
using Microsoft.EntityFrameworkCore;
using CapituloZero.SharedKernel;
using CapituloZero.Domain.Editora.Events;

namespace CapituloZero.Application.Editora.Etapas;

internal sealed class CompleteEtapaCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CompleteEtapaCommand>
{
    public async Task<Result> Handle(CompleteEtapaCommand command, CancellationToken cancellationToken)
    {
        Etapa? etapa = await context.Etapas
            .SingleOrDefaultAsync(e => e.Id == command.EtapaId, cancellationToken)
            .ConfigureAwait(false);

        if (etapa is null)
        {
            return Result.Failure(Error.NotFound("Etapa.NotFound", "Stage not found"));
        }

        etapa.Complete();
        etapa.Raise(new EtapaConcluidaDomainEvent(etapa.LivroId, etapa.Id));

        // Advance book if possible
        Livro? livro = await context.Livros
            .Include(l => l.Etapas)
            .SingleOrDefaultAsync(l => l.Id == etapa.LivroId, cancellationToken)
            .ConfigureAwait(false);

        if (livro is not null)
        {
            livro.Advance();
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
