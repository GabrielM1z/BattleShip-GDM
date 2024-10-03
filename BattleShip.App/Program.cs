using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BattleShip.App;
using Microsoft.Extensions.Options;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5195") });

builder.Services.Configure<GameStateOptions>(options =>
{
    options.Size = 10; // Définit la taille par défaut ici
});

// Enregistrer GameState en singleton
builder.Services.AddSingleton<GameState>(provider =>
{
    var options = provider.GetRequiredService<IOptions<GameStateOptions>>().Value;
    return new GameState(options.Size);
});

await builder.Build().RunAsync();
