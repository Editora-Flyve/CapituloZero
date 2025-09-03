using CapituloZero.Infra.IdentityApp;
using CapituloZero.MigrationService;
using CapituloZero.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddIdentityUser();

var host = builder.Build();
host.Run();