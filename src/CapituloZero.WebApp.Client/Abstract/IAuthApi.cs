using CapituloZero.WebApp.Client.Models;
using CapituloZero.WebApp.Client.Services;
using CapituloZero.WebApp.Client.Services.Result;

namespace CapituloZero.WebApp.Client.Abstract;

public interface IAuthApi
{
    Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<ApiResult<RegistrarResponse>> RegistrarAsync(RegistrarRequest request, CancellationToken ct = default);
}