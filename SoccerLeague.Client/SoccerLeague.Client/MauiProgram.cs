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

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Add Radzen services
        builder.Services.AddRadzenComponents();

        // Add HttpClient for API calls
        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7176/")//This is the backend API URL on my development machine
        });

        // Add Authentication Service
        builder.Services.AddScoped<AuthenticationService>();

        return builder.Build();
    }
}