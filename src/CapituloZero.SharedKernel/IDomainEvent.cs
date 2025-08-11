using System.Diagnostics.CodeAnalysis;

namespace CapituloZero.SharedKernel;

[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Marker interface for domain events to support dispatching and cross-cutting concerns.")]
public interface IDomainEvent;
