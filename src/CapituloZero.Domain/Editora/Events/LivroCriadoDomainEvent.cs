using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Events;

public sealed record LivroCriadoDomainEvent(Guid LivroId) : IDomainEvent;
