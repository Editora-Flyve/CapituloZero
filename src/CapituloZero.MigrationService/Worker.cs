using System.Diagnostics;
using CapituloZero.Infra.IdentityApp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.MigrationService
{
    public class Worker(
        IServiceProvider serviceProvider,
        IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
    {
        public const string ActivitySourceName = "Migrations";
        private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await RunMigrationAsync(dbContext, cancellationToken);


                var rm = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!(await rm.RoleExistsAsync("Cliente")))
                    await rm.CreateAsync(new IdentityRole("Cliente")
                    {
                        NormalizedName = "CLIENTE",
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                if (!(await rm.RoleExistsAsync("Autor")))
                    await rm.CreateAsync(new IdentityRole("Autor")
                    {
                        NormalizedName = "AUTOR",
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                if (!(await rm.RoleExistsAsync("Parceiro")))
                    await rm.CreateAsync(new IdentityRole("Parceiro")
                    {
                        NormalizedName = "PARCEIRO",
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                if (!(await rm.RoleExistsAsync("Admin")))
                    await rm.CreateAsync(new IdentityRole("Admin")
                    {
                        NormalizedName = "ADMIN",
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                //await SeedDataAsync(dbContext, cancellationToken);
            }
            catch (Exception ex)
            {
                activity?.AddException(ex);
                throw;
            }

            hostApplicationLifetime.StopApplication();
        }

        private static async Task RunMigrationAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // Run migration in a transaction to avoid partial migration if it fails.
                await dbContext.Database.MigrateAsync(cancellationToken);
            });
        }

        private static async Task SeedDataAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // Seed the database
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
                if (!await dbContext.Roles.AnyAsync(cancellationToken))
                {
                    await dbContext.Roles.AddAsync(new IdentityRole("Cliente"), cancellationToken);
                    await dbContext.Roles.AddAsync(new IdentityRole("Autor"), cancellationToken);
                    await dbContext.Roles.AddAsync(new IdentityRole("Parceiro"), cancellationToken);
                    await dbContext.Roles.AddAsync(new IdentityRole("Admin"), cancellationToken);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            });
        }
    }
}