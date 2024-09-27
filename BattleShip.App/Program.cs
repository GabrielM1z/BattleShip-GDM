using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BattleShip.App;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5195") });

// Enregistrer GameState en singleton
builder.Services.AddSingleton<GameState>();

await builder.Build().RunAsync();
