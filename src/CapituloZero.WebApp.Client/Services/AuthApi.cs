using System.Net.Http.Json;
using CapituloZero.WebApp.Client.Abstract;
using CapituloZero.WebApp.Client.Extensions;
using CapituloZero.WebApp.Client.Models;
using CapituloZero.WebApp.Client.Services.Result;

namespace CapituloZero.WebApp.Client.Services;

internal sealed class AuthApi : IAuthApi
{
    private readonly HttpClient _http;
    private readonly IAntiforgeryTokenProvider _anti;

    public AuthApi(HttpClient http, IAntiforgeryTokenProvider anti)
    {
        _http = http;
        _anti = anti;
    }

    public async Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var token = await _anti.GetTokenAsync(ct);
        var msg = new HttpRequestMessage(HttpMethod.Post, "/Account/LoginApi")
        {
            Content = JsonContent.Create(request)
        };
        msg.AddAntiforgeryHeaderIfPresent(token);
        return await Send<LoginResponse>(msg, ct);
    }

    public async Task<ApiResult<RegistrarResponse>> RegistrarAsync(RegistrarRequest request, CancellationToken ct = default)
    {
        var token = await _anti.GetTokenAsync(ct);
        var msg = new HttpRequestMessage(HttpMethod.Post, "/Account/RegisterApi")
        {
            Content = JsonContent.Create(request)
        };
        msg.AddAntiforgeryHeaderIfPresent(token);
        return await Send<RegistrarResponse>(msg, ct);
    }

    private async Task<ApiResult<T>> Send<T>(HttpRequestMessage msg, CancellationToken ct)
    {
        try
        {
            var resp = await _http.SendAsync(msg, ct);
            if(!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(ct);
                return ApiResult<T>.Fail(((int)resp.StatusCode).ToString(), body);
            }
            var payload = await resp.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
            return payload is null 
                ? ApiResult<T>.Fail("EMPTY_RESPONSE", "Resposta vazia.")
                : ApiResult<T>.Ok(payload);
        }
        catch(Exception ex)
        {
            return ApiResult<T>.Fail("EXCEPTION", ex.Message);
        }
    }
}