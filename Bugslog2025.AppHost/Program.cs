using Bugslog2025.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddGarnet("cache")
    .WithLifetime(ContainerLifetime.Persistent);

var seq = builder.AddSeq("seq")
                 .ExcludeFromManifest()
                 .WithLifetime(ContainerLifetime.Persistent)
                 .WithEnvironment("ACCEPT_EULA", "Y");

var apiService = builder.AddProject<Projects.Bugslog2025_ApiService>("apiservice")
    .WithSwaggerUI()
    .WithReference(seq);

builder.AddProject<Projects.Bugslog2025_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(seq)
    .WaitFor(seq);

builder.Build().Run();
