using BitzArt.Blazor.Cookies;
using Blazored.LocalStorage;
using Frank.IdentityServer.LoginApp.Components;
using Frank.IdentityServer.LoginApp.Models;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

// PII in logging is useful for debugging, but should be disabled in production:        
IdentityModelEventSource.ShowPII = true;
IdentityModelEventSource.LogCompleteSecurityArtifact = true;
builder.AddServiceDefaults();

builder.Services.AddAuthentication()
    .AddCookie("oidc", options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/accessdenied";
        options.ReturnUrlParameter = "returnUrl";
        options.Cookie.Name = "Frank.IdentityServer.LoginApp.Cookie";
    });
    
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddBlazoredLocalStorage();
builder.AddBlazorCookies();

builder.Services.Configure<LoginConfiguration>(builder.Configuration.GetSection(nameof(LoginConfiguration)));

builder.Services.AddHttpClient();
builder.Services.ConfigureHttpClientDefaults(x =>
{
    // Configure the HttpClient for the Home component
    x.ConfigureHttpClient(y =>
    {
        y.BaseAddress = new Uri(builder.Configuration["LoginConfiguration:Authority"] ?? "https://localhost:6001");
    });
});

builder.Services.AddHttpLogging();

var app = builder.Build();
app.UseHttpLogging();
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();