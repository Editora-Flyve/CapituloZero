using System.Diagnostics.CodeAnalysis;

namespace CapituloZero.SharedKernel;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Following ubiquitous language; aligns with domain events pipeline.")]
public interface IDomainEventHandler<in T> where T : IDomainEvent
{
    Task Handle(T domainEvent, CancellationToken cancellationToken);
}
