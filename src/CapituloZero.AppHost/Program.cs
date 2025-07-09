var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.CapituloZero_Web_Api>("api");

builder.AddProject<Projects.CapituloZero_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
