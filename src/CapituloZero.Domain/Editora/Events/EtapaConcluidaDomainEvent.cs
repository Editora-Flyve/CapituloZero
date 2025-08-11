using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Events;

public sealed record EtapaConcluidaDomainEvent(Guid LivroId, Guid EtapaId) : IDomainEvent;
