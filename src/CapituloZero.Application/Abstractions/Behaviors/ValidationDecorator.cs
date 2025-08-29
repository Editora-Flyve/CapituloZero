using CapituloZero.Application.Abstractions.Messaging;
using FluentValidation;
using FluentValidation.Results;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Abstractions.Behaviors;

internal static class ValidationDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(command, validators).ConfigureAwait(false);

            if (validationFailures.Length == 0)
            {
                return await innerHandler.Handle(command, cancellationToken).ConfigureAwait(false);
            }

            return Result.Failure<TResponse>(CreateValidationError(validationFailures));
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(command, validators).ConfigureAwait(false);

            if (validationFailures.Length == 0)
            {
                return await innerHandler.Handle(command, cancellationToken).ConfigureAwait(false);
            }

            return Result.Failure(CreateValidationError(validationFailures));
        }
    }

    private static async Task<ValidationFailure[]> ValidateAsync<TCommand>(
        TCommand command,
        IEnumerable<IValidator<TCommand>> validators)
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TCommand>(command);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context))).ConfigureAwait(false);

        ValidationFailure[] validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();

        return validationFailures;
    }

    private static ValidationErrorInternal CreateValidationError(ValidationFailure[] validationFailures) =>
        new(validationFailures.Select(f => ErrorInternal.Problem(f.ErrorCode, f.ErrorMessage)).ToArray());
}
