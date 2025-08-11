using System.Diagnostics.CodeAnalysis;

namespace CapituloZero.Application.Abstractions.Messaging;

[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Marker interface for pipeline behaviors and DI registration.")]
public interface ICommand;

[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Marker interface for pipeline behaviors and DI registration.")]
[SuppressMessage("Usage", "S2326:'TResponse' is not used in the interface", Justification = "Marker interface generic for strong typing through the pipeline.")]
public interface ICommand<TResponse>;
