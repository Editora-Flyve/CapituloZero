namespace CapituloZero.Web.Api.Endpoints.Editora.Fluxos;

internal sealed class CreateFluxoProducaoRequest
{
    public required string Nome { get; set; }
    public string? Descricao { get; set; }
}
