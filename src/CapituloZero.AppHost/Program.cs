var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin()
                      .WithDataVolume(isReadOnly: false);

var postgresdb = postgres.AddDatabase("postgresdb");

var api = builder.AddProject<Projects.CapituloZero_Web_Api>("api")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.CapituloZero_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

await builder.Build().RunAsync().ConfigureAwait(false);
