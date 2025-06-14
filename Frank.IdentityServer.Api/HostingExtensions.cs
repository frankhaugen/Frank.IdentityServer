using Duende.IdentityModel;
using Duende.IdentityServer.Configuration;
using Frank.IdentityServer.Api.Middleware;
using Microsoft.IdentityModel.Logging; // Add this using statement

namespace Frank.IdentityServer.Api;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        // PII in logging is useful for debugging, but should be disabled in production:        
        IdentityModelEventSource.ShowPII = true;
        IdentityModelEventSource.LogCompleteSecurityArtifact = true;

        builder.Services.AddRazorPages();

        var isBuilder = builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/#state-hashing
                options.EmitStateHash = true;
            })
            .AddDeveloperSigningCredential() // this is for development only, don't use in production
            .AddTestUsers(TestUsers.Users);

        // in-memory, code config
        isBuilder.AddInMemoryIdentityResources(Config.IdentityResources);
        isBuilder.AddInMemoryApiScopes(Config.ApiScopes);
        isBuilder.AddInMemoryClients(Config.Clients);
        
        // Grant validation
        isBuilder.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

        // if you want to use server-side sessions: https://blog.duendesoftware.com/posts/20220406_session_management/
        // then enable it
        //isBuilder.AddServerSideSessions();
        //
        // and put some authorization on the admin/management pages
        //builder.Services.AddAuthorization(options =>
        //       options.AddPolicy("admin",
        //           policy => policy.RequireClaim("sub", "1"))
        //   );
        //builder.Services.Configure<RazorPagesOptions>(options =>
        //    options.Conventions.AuthorizeFolder("/ServerSideSessions", "admin"));

        builder.Services.AddAuthentication();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.MapDefaultEndpoints();
        app.UseRequestResponseLogging(); // Add this line early in the pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        // app.MapRazorPages()
        //     .RequireAuthorization();

        return app;
    }
}