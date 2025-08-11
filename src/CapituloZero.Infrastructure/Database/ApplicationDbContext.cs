using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using CapituloZero.SharedKernel;
using CapituloZero.Domain.Todos.Entities;
using CapituloZero.Domain.Users.Entities;
using CapituloZero.Domain.Editora.Entities;

namespace CapituloZero.Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<TodoItem> TodoItems { get; set; }

    public DbSet<Autor> Autores { get; set; }
    public DbSet<Livro> Livros { get; set; }
    public DbSet<Etapa> Etapas { get; set; }
    public DbSet<Terceiro> Terceiros { get; set; }
    public DbSet<Funcao> Funcoes { get; set; }
    public DbSet<FluxoProducao> FluxosProducao { get; set; }
    public DbSet<EtapaTemplate> EtapasTemplate { get; set; }
    public DbSet<Artefato> Artefatos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // When should you publish domain events?
        //
        // 1. BEFORE calling SaveChangesAsync
        //     - domain events are part of the same transaction
        //     - immediate consistency
        // 2. AFTER calling SaveChangesAsync
        //     - domain events are a separate transaction
        //     - eventual consistency
        //     - handlers can fail

    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    await PublishDomainEventsAsync().ConfigureAwait(false);

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.DomainEvents.ToList();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

    await domainEventsDispatcher.DispatchAsync(domainEvents).ConfigureAwait(false);
    }
}
