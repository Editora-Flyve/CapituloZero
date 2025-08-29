using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.SharedKernel;

namespace CapituloZero.ApplicationTests.Helpers;

public sealed class FakeUserContext : IUserContext
{
    public Guid UserId { get; set; }
}

public sealed class StubDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow { get; set; }
}

