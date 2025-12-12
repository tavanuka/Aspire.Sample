using Sample.AppHost.Extensions;
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgWeb()
    .WithLifetime(ContainerLifetime.Persistent);

var coreDb = postgres.AddDatabase("core-db");

var cache = builder.AddRedis("cache")
    .WithClearCommand(); // Custom command that flushes the current cache


// Migration service that separates the concern for population of the database and applying migrations.
var migrationWorker = builder.AddProject<Projects.Sample_MigrationWorker>("migration-worker")
    .WithReference(coreDb)
    .WaitFor(coreDb);

var apiService = builder.AddProject<Projects.Sample_ApiService>("apiservice")
    .WithReference(migrationWorker)
    .WaitForCompletion(migrationWorker)  // Ensures database is populated and correctly setup before starting the service
	.WithReference(coreDb)
	.WaitFor(coreDb)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Sample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

// An UI representation of all available endpoints for the web services
var scalar = builder.AddScalarApiReference();
scalar.WithApiReference(apiService);

builder.Build().Run();
