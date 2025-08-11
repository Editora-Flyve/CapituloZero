using FluentValidation;

namespace CapituloZero.Application.Editora.Etapas;

internal sealed class SubmitArtefatoCommandValidator : AbstractValidator<SubmitArtefatoCommand>
{
    public SubmitArtefatoCommandValidator()
    {
        RuleFor(x => x.EtapaId).NotEmpty();
        RuleFor(x => x.FileUri)
            .NotNull()
            .Must(uri => uri.IsAbsoluteUri)
            .WithMessage("FileUri must be an absolute URI.");
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(127);
        RuleFor(x => x.SizeBytes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UploadedByUserId).NotEmpty();
    }
}
