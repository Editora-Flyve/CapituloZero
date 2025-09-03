using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace CapituloZero.Infra.IdentityApp
{
    public static class DIExtensions
    {
        public static IHostApplicationBuilder AddIdentityUser(this IHostApplicationBuilder builder)
        {
            builder.AddNpgsqlDbContext<ApplicationDbContext>(connectionName: "capitulozero-db",
                configureDbContextOptions: optionsBuilder =>
                {
                    optionsBuilder.UseNpgsql(b =>
                    {
                        b.MigrationsAssembly("CapituloZero.Infra.IdentityApp");
                    });
                } );
        
            return builder;
        }
    }
}