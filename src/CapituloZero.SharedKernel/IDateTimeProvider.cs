namespace CapituloZero.SharedKernel;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
