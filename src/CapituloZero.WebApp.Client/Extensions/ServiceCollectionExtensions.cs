using CapituloZero.WebApp.Client.Abstract;
using CapituloZero.WebApp.Client.Services;

namespace CapituloZero.WebApp.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClientAppServices(this IServiceCollection services)
    {
        services.AddScoped<IAntiforgeryTokenProvider, AntiforgeryTokenProvider>();
        services.AddScoped<IAuthApi, AuthApi>();
        return services;
    }
}
