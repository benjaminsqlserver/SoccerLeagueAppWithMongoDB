using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using SoccerLeague.Client.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add Radzen services
builder.Services.AddRadzenComponents();

// Configure HttpClient for BFF calls - points to the Web server
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// Register Authentication Service (BFF mode for Web)
builder.Services.AddScoped<AuthenticationService>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new AuthenticationService(httpClient); // No apiBaseUrl = BFF mode
});

await builder.Build().RunAsync();
