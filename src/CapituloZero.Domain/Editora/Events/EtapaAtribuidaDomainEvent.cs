using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Events;

public sealed record EtapaAtribuidaDomainEvent(Guid LivroId, Guid EtapaId, Guid TerceiroId) : IDomainEvent;
