using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Editora.Etapas;

public sealed record SubmitArtefatoCommand(
    Guid EtapaId,
    string FileUri,
    string FileName,
    string ContentType,
    long SizeBytes,
    Guid UploadedByUserId) : ICommand;
