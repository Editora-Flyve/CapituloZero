using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CapituloZero.WebApp.Client.Abstract;

namespace CapituloZero.WebApp.Client.Services;

internal sealed class AntiforgeryTokenProvider : IAntiforgeryTokenProvider
{
    private readonly HttpClient _http;
    private string? _cached;
    private readonly SemaphoreSlim _lock = new(1,1);

    public AntiforgeryTokenProvider(HttpClient http) => _http = http;

    public async Task<string?> GetTokenAsync(CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(_cached)) return _cached;

        await _lock.WaitAsync(ct);
        try
        {
            if (!string.IsNullOrWhiteSpace(_cached)) return _cached;
            var resp = await _http.GetFromJsonAsync<AntiforgeryResponse>("/antiforgery", ct);
            _cached = resp?.token;
            return _cached;
        }
        catch
        {
            return null; // tolerante: ausência do token tratada pelos endpoints
        }
        finally
        {
            _lock.Release();
        }
    }

    public record AntiforgeryResponse(string token);
}
