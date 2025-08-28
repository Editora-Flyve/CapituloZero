using FluentValidation;

namespace CapituloZero.Application.Users.AddTipos;

internal sealed class AddTiposCommandValidator : AbstractValidator<AddTiposCommand>
{
    public AddTiposCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CurrentUserId).NotEmpty();
        RuleFor(x => x.Tipos).NotNull();
    }
}
