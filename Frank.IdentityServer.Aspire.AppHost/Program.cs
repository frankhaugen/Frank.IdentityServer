using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var identityServer = builder.AddProject<Frank_IdentityServer_Api>("identityServer");

var loginApp = builder.AddProject<Frank_IdentityServer_LoginApp>("loginApp")
    .WithReference(identityServer);

builder.Build().Run();