namespace CapituloZero.WebApp.Client.Abstract;

public interface IAntiforgeryTokenProvider
{
    Task<string?> GetTokenAsync(CancellationToken ct = default);
}