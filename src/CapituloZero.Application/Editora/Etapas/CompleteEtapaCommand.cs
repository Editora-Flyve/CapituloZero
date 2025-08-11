using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Editora.Etapas;

public sealed record CompleteEtapaCommand(Guid EtapaId) : ICommand;
