using Microsoft.Extensions.Logging;
using Radzen;
using SoccerLeague.Client.Shared.Services;

namespace SoccerLeague.Client;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add Blazor WebView
        builder.Services.AddMauiBlazorWebView();

        // Add Radzen services
        builder.Services.AddRadzenComponents();

        // Configure HttpClient for direct API calls
        var apiBaseUrl = "https://localhost:7176/api"; // Update for production

        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(apiBaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        });

        // Register Authentication Service (Direct API mode for MAUI)
        builder.Services.AddScoped(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            return new AuthenticationService(httpClient, apiBaseUrl);
        });

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
