using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Events;

public sealed record ArtefatoSubmetidoDomainEvent(Guid EtapaId, string FileUri) : IDomainEvent;
