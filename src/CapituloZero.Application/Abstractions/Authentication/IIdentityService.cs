using CapituloZero.Application.Users.GetByEmail;
using CapituloZero.Application.Users.GetById;
using CapituloZero.Application.Users.Login;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Abstractions.Authentication;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterAsync(string email, string firstName, string lastName, string password, CancellationToken ct = default);
    Task<Result<LoginResponse>> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<Result<LoginResponse>> RefreshAsync(string refreshToken, CancellationToken ct = default);
    Task<Result<CapituloZero.Application.Users.GetById.UserResponse>> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default);
    Task<Result<CapituloZero.Application.Users.GetByEmail.UserResponse>> GetByEmailAsync(string email, Guid currentUserId, CancellationToken ct = default);
    Task<Result> AddUserTypesAsync(Guid userId, IEnumerable<string> tipos, CancellationToken ct = default);
}
