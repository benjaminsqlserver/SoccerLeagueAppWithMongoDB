using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using SoccerLeague.Client.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add Radzen services
builder.Services.AddRadzenComponents();

// Add HttpClient for BFF calls - points to the Web server (BFF)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) // This points to the BFF server
});

// Register BFF Authentication Service instead of direct API calls
builder.Services.AddScoped<BffAuthenticationService>();

// Optional: Keep the old service for MAUI if you want to share the project
// You can use conditional compilation or DI to choose the right one
#if WEBASSEMBLY
builder.Services.AddScoped<AuthenticationService, BffAuthenticationService>();
#endif

await builder.Build().RunAsync();