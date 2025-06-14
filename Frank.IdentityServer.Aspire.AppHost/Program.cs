using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var identityServer = builder.AddProject<Frank_IdentityServer_Api>("identityServer");

var loginApp = builder.AddProject<Frank_IdentityServer_LoginApp>("loginApp")
    .WithReference(identityServer)
    .WaitFor(identityServer);

var blazorApp = builder.AddProject<Frank_IdentityServer_Clients_BlazorClientApp>("blazorApp")
    .WithReference(loginApp)
    .WaitFor(loginApp);

// var mauiApp = builder.AddProject("mauiApp", "../Frank.IdentityServer.Clients.MauiClientApp/Frank.IdentityServer.Clients.MauiClientApp.csproj")
//     .WithReference(loginApp)
//     .WaitFor(loginApp);

// var avaloniaApp = builder.AddProject<Frank_IdentityServer_Clients_AvaloniaClientApp>("avaloniaApp")
//     .WithReference(loginApp)
//     .WaitFor(loginApp);

builder.Build().Run();