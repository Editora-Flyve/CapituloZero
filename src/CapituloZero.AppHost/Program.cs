var builder = DistributedApplication.CreateBuilder(args);

var postgresdb = builder.AddPostgres("postgres")
                      .WithLifetime(ContainerLifetime.Persistent)
                      .WithPgWeb()
                      .WithDataVolume(isReadOnly: false)
                      .AddDatabase("postgresdb");

var api = builder.AddProject<Projects.CapituloZero_Web_Api>("api")
    // Provide connection strings with the exact keys the API expects
    .WithEnvironment("ConnectionStrings__database", postgresdb)
    .WithEnvironment("ConnectionStrings__postgresdb", postgresdb)
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.CapituloZero_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

await builder.Build().RunAsync().ConfigureAwait(false);
