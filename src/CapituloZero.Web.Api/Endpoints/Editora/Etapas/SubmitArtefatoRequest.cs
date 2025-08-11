namespace CapituloZero.Web.Api.Endpoints.Editora.Etapas;

internal sealed class SubmitArtefatoRequest
{
    public required Guid EtapaId { get; set; }
    public required string FileUri { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public long SizeBytes { get; set; }
    public required Guid UploadedByUserId { get; set; }
}
