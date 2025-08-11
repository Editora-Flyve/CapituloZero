using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Events;

public sealed record LivroAvancouDomainEvent(Guid LivroId, int CurrentEtapaIndex) : IDomainEvent;
