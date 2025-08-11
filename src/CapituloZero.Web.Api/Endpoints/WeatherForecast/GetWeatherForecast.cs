
namespace CapituloZero.Web.Api.Endpoints.WeatherForecast;

internal sealed class GetWeatherForecast : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

        app.MapGet("/weatherforecast", () =>
        {
            // Use a cryptographically secure RNG to avoid CA5394
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            int NextInt(int minValue, int maxValue)
            {
                // inclusive min, exclusive max
                Span<byte> fourBytes = stackalloc byte[4];
                rng.GetBytes(fourBytes);
                int value = Math.Abs(BitConverter.ToInt32(fourBytes));
                return minValue + (value % (maxValue - minValue));
            }

            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    NextInt(-20, 55),
                    summaries[NextInt(0, summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithTags(Tags.WeatherForecasts);
    }
}

file sealed record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
