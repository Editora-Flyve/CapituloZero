using FluentValidation;

namespace CapituloZero.Application.Editora.Etapas;

internal sealed class AssignTerceiroToEtapaCommandValidator : AbstractValidator<AssignTerceiroToEtapaCommand>
{
    public AssignTerceiroToEtapaCommandValidator()
    {
        RuleFor(x => x.EtapaId).NotEmpty();
        RuleFor(x => x.TerceiroId).NotEmpty();
    }
}
