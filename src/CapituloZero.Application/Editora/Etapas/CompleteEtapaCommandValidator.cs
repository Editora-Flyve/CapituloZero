using FluentValidation;

namespace CapituloZero.Application.Editora.Etapas;

internal sealed class CompleteEtapaCommandValidator : AbstractValidator<CompleteEtapaCommand>
{
    public CompleteEtapaCommandValidator()
    {
        RuleFor(x => x.EtapaId).NotEmpty();
    }
}
