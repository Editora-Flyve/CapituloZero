﻿using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Users.Register;

public sealed record RegisterUserCommand(string Email, string FirstName, string LastName, string Password)
    : ICommand<Guid>;
