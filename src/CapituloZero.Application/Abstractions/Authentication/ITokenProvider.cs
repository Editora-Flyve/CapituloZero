using CapituloZero.Domain.Users;

namespace CapituloZero.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(User user);
    string Create(Guid userId, string email, IEnumerable<string> roles);
}
