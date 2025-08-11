using FluentValidation;

namespace CapituloZero.Application.Editora.Fluxos;

internal sealed class CreateFluxoProducaoCommandValidator : AbstractValidator<CreateFluxoProducaoCommand>
{
    public CreateFluxoProducaoCommandValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descricao).MaximumLength(2000);
    }
}
