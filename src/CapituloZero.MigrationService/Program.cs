using CapituloZero.Infra.IdentityApp;
using CapituloZero.MigrationService;
using CapituloZero.ServiceDefaults;
using Microsoft.AspNetCore.Identity;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddIdentityUser();

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var host = builder.Build();
host.Run();