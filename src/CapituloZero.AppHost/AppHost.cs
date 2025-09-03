var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres-capitulozero")
    .WithPgWeb()
    .WithDataVolume(isReadOnly: false);
var postgresdb = postgres.AddDatabase("capitulozero-db");

var migrations = builder.AddProject<Projects.CapituloZero_MigrationService>("capitulozero-migrationservice")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);


builder.AddProject<Projects.CapituloZero_WebApp>("capitulozero-webapp")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithReference(migrations)
    .WaitForCompletion(migrations);

builder.Build().Run();
