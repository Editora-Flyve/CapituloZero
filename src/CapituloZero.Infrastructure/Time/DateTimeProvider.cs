using CapituloZero.SharedKernel;

namespace CapituloZero.Infrastructure.Time;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    private readonly TimeProvider _timeProvider;

    public DateTimeProvider(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public DateTime UtcNow => _timeProvider.GetUtcNow().UtcDateTime;
}
