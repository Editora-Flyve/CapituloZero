using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Domain.Editora.Entities;
using Microsoft.EntityFrameworkCore;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Editora.Livros;

internal sealed class CreateLivroCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateLivroCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateLivroCommand command, CancellationToken cancellationToken)
    {
        FluxoProducao? fluxo = await context.FluxosProducao
            .Include(f => f.Etapas)
            .SingleOrDefaultAsync(f => f.Id == command.FluxoProducaoId, cancellationToken)
            .ConfigureAwait(false);

        if (fluxo is null)
        {
            return Result.Failure<Guid>(Error.NotFound("Fluxo.NotFound", "Production pipeline not found"));
        }

        var autor = new Autor
        {
            Nome = command.AutorNome,
            Email = command.AutorEmail,
            Livros = []
        };

        var livro = new Livro
        {
            Titulo = command.Titulo,
            Subtitulo = command.Subtitulo ?? string.Empty,
            Autor = autor
        };

        livro.InitializeFromTemplate(fluxo);

        context.Autores.Add(autor);
        context.Livros.Add(livro);

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return livro.Id;
    }
}
