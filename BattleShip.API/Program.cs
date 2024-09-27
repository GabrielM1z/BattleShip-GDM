using System.Reflection.Metadata.Ecma335;
using BattleShip.API.Service; // Assure-toi que le chemin d'importation est correct
using BattleShip.Models;

var builder = WebApplication.CreateBuilder(args);

// Ajoute le service GridService au conteneur DI
builder.Services.AddSingleton<GridService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Route Hello World
app.MapGet("/", () => "Hello World")
.WithOpenApi();


// Route pour générer deux grilles
app.MapGet("/generateGrid", (GridService gridService) =>
{
    // Création de la grille pour le joueur 1
    Grid gridJ1 = gridService.CreateGrid();
    char[][] gridArrayJ1 = gridService.GetGridArray(gridJ1); // Conversion de la grille en tableau de tableaux

    // Impression de la grille J1 dans la console
    gridService.PrintGrid(gridArrayJ1,"gridJ1");

    // Création de la grille pour le joueur 2
    Grid gridJ2 = gridService.CreateGrid();
    char[][] gridArrayJ2 = gridService.GetGridArray(gridJ2); // Conversion de la grille en tableau de tableaux

    // Impression de la grille J2 dans la console
    gridService.PrintGrid(gridArrayJ2,"gridJ2");

    // Retourne les grilles sous forme de JSON
    return Results.Ok(new { gridJ1 = gridArrayJ1, gridJ2 = gridArrayJ2 });
})
.WithOpenApi();



app.Run();
