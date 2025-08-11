using FluentValidation;

namespace CapituloZero.Application.Editora.Livros;

internal sealed class CreateLivroCommandValidator : AbstractValidator<CreateLivroCommand>
{
    public CreateLivroCommandValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(250);
        RuleFor(x => x.AutorNome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AutorEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.FluxoProducaoId).NotEmpty();
    }
}
