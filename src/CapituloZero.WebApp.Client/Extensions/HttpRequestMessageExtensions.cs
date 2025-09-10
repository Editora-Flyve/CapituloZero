namespace CapituloZero.WebApp.Client.Extensions;

internal static class HttpRequestMessageExtensions
{
    public static void AddAntiforgeryHeaderIfPresent(this HttpRequestMessage msg, string? token)
    {
        if(!string.IsNullOrWhiteSpace(token))
            msg.Headers.Add("RequestVerificationToken", token);
    }
}