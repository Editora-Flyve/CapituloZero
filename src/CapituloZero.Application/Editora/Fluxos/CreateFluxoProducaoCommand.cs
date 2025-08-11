using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Editora.Fluxos;

public sealed record CreateFluxoProducaoCommand(string Nome, string? Descricao) : ICommand<Guid>;
