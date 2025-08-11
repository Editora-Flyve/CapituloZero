using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Domain.Users;

namespace CapituloZero.Application.Users.Register;

public sealed record RegisterUserCommand(string Email, string FirstName, string LastName, string Password, UserType? Types = null)
    : ICommand<Guid>;
