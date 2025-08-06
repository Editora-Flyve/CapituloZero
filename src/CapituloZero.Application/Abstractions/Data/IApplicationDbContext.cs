using CapituloZero.Domain.Todos.Entities;
using CapituloZero.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<TodoItem> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
