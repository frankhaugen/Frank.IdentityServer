using BitzArt.Blazor.Cookies;
using Blazored.LocalStorage;
using Frank.IdentityServer.LoginApp.Components;
using Frank.IdentityServer.LoginApp.Components.Pages;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddBlazoredLocalStorage();
builder.AddBlazorCookies();

builder.Services.AddHttpClient();
builder.Services.ConfigureHttpClientDefaults(x =>
{
    // Configure the HttpClient for the Home component
    x.ConfigureHttpClient(y =>
    {
        y.BaseAddress = new Uri(builder.Configuration["LoginConfiguration:Authority"] ?? "https://localhost:6001");
    });
});

var app = builder.Build();

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

app.MapDefaultEndpoints();

app.Run();