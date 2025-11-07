using SoccerLeague.Client.Web.Components;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Add Radzen services
builder.Services.AddRadzenComponents();

// Add HttpClient Factory for BFF
builder.Services.AddHttpClient();

// Add Controllers for BFF API
builder.Services.AddControllers();

// Add CORS if needed (optional, for development)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm", policy =>
    {
        policy.WithOrigins("https://localhost:7176")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Add CORS
app.UseCors("AllowBlazorWasm");

// Map controllers for BFF
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(SoccerLeague.Client.Shared._Imports).Assembly,
        typeof(SoccerLeague.Client.Web.Client._Imports).Assembly);

app.Run();