using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Entities;

public class Artefato : Entity
{
    public required Guid EtapaId { get; set; }
    public required string FileUri { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public long SizeBytes { get; set; }
    public Guid UploadedByUserId { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
