using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Editora.Livros;

public sealed record CreateLivroCommand(
    string Titulo,
    string? Subtitulo,
    string AutorNome,
    string AutorEmail,
    Guid FluxoProducaoId) : ICommand<Guid>;
