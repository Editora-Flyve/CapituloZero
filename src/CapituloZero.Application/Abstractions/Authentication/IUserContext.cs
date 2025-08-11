namespace CapituloZero.Application.Abstractions.Authentication;

public interface IUserContext
{
    Guid UserId { get; }
    string? ActiveType { get; }
}
