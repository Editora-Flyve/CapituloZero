namespace CapituloZero.Web.Api.Endpoints.Editora.Etapas;

internal sealed class AssignTerceiroToEtapaRequest
{
    public required Guid EtapaId { get; set; }
    public required Guid TerceiroId { get; set; }
}
