using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Events;

public sealed record ArtefatoSubmetidoDomainEvent(Guid EtapaId, Uri FileUri) : IDomainEvent;
