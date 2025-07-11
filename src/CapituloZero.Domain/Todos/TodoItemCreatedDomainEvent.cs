﻿using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Todos;

public sealed record TodoItemCreatedDomainEvent(Guid TodoItemId) : IDomainEvent;
