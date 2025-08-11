using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Events;

public sealed record LivroConcluidoDomainEvent(Guid LivroId) : IDomainEvent;
