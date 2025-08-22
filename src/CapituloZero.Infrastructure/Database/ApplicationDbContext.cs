using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Infrastructure.DomainEvents;
using CapituloZero.Domain.Todos;
using Microsoft.EntityFrameworkCore;
using CapituloZero.SharedKernel;
using CapituloZero.Infrastructure.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CapituloZero.Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options), IApplicationDbContext
{
    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

    ArgumentNullException.ThrowIfNull(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.HasDefaultSchema(Schemas.Default);

        // Map Identity tables to the usuarios schema with snake_case names
        builder.Entity<ApplicationUser>(b =>
        {
            b.ToTable("users", Schemas.Usuarios);
        });

        builder.Entity<ApplicationRole>(b =>
        {
            b.ToTable("roles", Schemas.Usuarios);
            b.HasData(
                new ApplicationRole { Id = UserTypeIds.Default, Name = UserTypes.Default, NormalizedName = UserTypes.Default.ToUpperInvariant(), ConcurrencyStamp = "seed" },
                new ApplicationRole { Id = UserTypeIds.Autor, Name = UserTypes.Autor, NormalizedName = UserTypes.Autor.ToUpperInvariant(), ConcurrencyStamp = "seed" },
                new ApplicationRole { Id = UserTypeIds.Parceiro, Name = UserTypes.Parceiro, NormalizedName = UserTypes.Parceiro.ToUpperInvariant(), ConcurrencyStamp = "seed" },
                new ApplicationRole { Id = UserTypeIds.Admin, Name = UserTypes.Admin, NormalizedName = UserTypes.Admin.ToUpperInvariant(), ConcurrencyStamp = "seed" }
            );
        });

        builder.Entity<IdentityUserRole<Guid>>(b =>
        {
            b.ToTable("user_roles", Schemas.Usuarios);
        });

        builder.Entity<IdentityUserClaim<Guid>>(b =>
        {
            b.ToTable("user_claims", Schemas.Usuarios);
        });

        builder.Entity<IdentityUserLogin<Guid>>(b =>
        {
            b.ToTable("user_logins", Schemas.Usuarios);
        });

        builder.Entity<IdentityRoleClaim<Guid>>(b =>
        {
            b.ToTable("role_claims", Schemas.Usuarios);
        });

        builder.Entity<IdentityUserToken<Guid>>(b =>
        {
            b.ToTable("user_tokens", Schemas.Usuarios);
        });
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
                List<IDomainEvent> domainEvents = entity.DomainEvents;

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

    await domainEventsDispatcher.DispatchAsync(domainEvents).ConfigureAwait(false);
    }
}
