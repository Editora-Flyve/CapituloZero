var builder = DistributedApplication.CreateBuilder(args);

var postgresdb = builder.AddPostgres("postgres")
                      .WithLifetime(ContainerLifetime.Persistent)
                      .WithPgAdmin()
                      .WithDataVolume(isReadOnly: false)
                      .AddDatabase("postgresdb");

var api = builder.AddProject<Projects.CapituloZero_Web_Api>("api")
    .WithExternalHttpEndpoints()
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.CapituloZero_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

await builder.Build().RunAsync().ConfigureAwait(false);
