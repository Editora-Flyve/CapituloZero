using FluentValidation;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Todos.Create;

public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Priority).IsInEnum();
        RuleFor(c => c.Description).NotEmpty().MaximumLength(255);
        RuleFor(c => c.DueDate)
            .GreaterThanOrEqualTo(_ => dateTimeProvider.UtcNow.Date)
            .When(x => x.DueDate.HasValue);
    }
}
