using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Editora.Etapas;

public sealed record AssignTerceiroToEtapaCommand(Guid EtapaId, Guid TerceiroId) : ICommand;
