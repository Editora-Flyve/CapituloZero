using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Domain.Editora.Entities;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Editora.Fluxos;

internal sealed class CreateFluxoProducaoCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateFluxoProducaoCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateFluxoProducaoCommand command, CancellationToken cancellationToken)
    {
        var fluxo = new FluxoProducao
        {
            Nome = command.Nome,
            Descricao = command.Descricao ?? string.Empty
        };

        context.FluxosProducao.Add(fluxo);

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return fluxo.Id;
    }
}
