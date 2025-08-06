using CapituloZero.Domain.Users.Entities;

namespace CapituloZero.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(User user);
}
