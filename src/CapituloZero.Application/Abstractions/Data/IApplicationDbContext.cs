using CapituloZero.Domain.Todos.Entities;
using CapituloZero.Domain.Users.Entities;
using CapituloZero.Domain.Editora.Entities;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<TodoItem> TodoItems { get; }
    DbSet<Autor> Autores { get; }
    DbSet<Livro> Livros { get; }
    DbSet<Etapa> Etapas { get; }
    DbSet<Terceiro> Terceiros { get; }
    DbSet<Funcao> Funcoes { get; }
    DbSet<FluxoProducao> FluxosProducao { get; }
    DbSet<EtapaTemplate> EtapasTemplate { get; }
    DbSet<Artefato> Artefatos { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
