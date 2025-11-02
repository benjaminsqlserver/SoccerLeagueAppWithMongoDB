using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using SoccerLeague.Client.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add Radzen services
builder.Services.AddRadzenComponents();

// Add HttpClient for API calls
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7176/")
});

// Add Authentication Service
builder.Services.AddScoped<AuthenticationService>();

await builder.Build().RunAsync();