namespace CapituloZero.Web.Api.Endpoints.Editora.Livros;

internal sealed class CreateLivroRequest
{
    public required string Titulo { get; set; }
    public string? Subtitulo { get; set; }
    public required string AutorNome { get; set; }
    public required string AutorEmail { get; set; }
    public required Guid FluxoProducaoId { get; set; }
}
