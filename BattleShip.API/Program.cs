using System.Reflection.Metadata.Ecma335;
using BattleShip.API.Service; // Assure-toi que le chemin d'importation est correct
using BattleShip.Models;
using Microsoft.AspNetCore.Mvc;

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

char[][] gridArrayJ1 = null;
char[][] gridArrayJ2 = null;

app.MapGet("/start", (GridService gridService) =>
{
    // Création de la grille pour le joueur 1
    Grid gridJ1 = gridService.CreateGrid();
    gridArrayJ1 = gridService.GetGridArray(gridJ1); // Conversion de la grille en tableau de tableaux

    // Impression de la grille J1 dans la console
    gridService.PrintGrid(gridArrayJ1, "gridJ1");

    // Création de la grille pour le joueur 2
    Grid gridJ2 = gridService.CreateGrid();
    gridArrayJ2 = gridService.GetGridArray(gridJ2); // Conversion de la grille en tableau de tableaux

    // Impression de la grille J2 dans la console
    gridService.PrintGrid(gridArrayJ2, "gridJ2");

    // Retourne les grilles sous forme de JSON
    return Results.Ok(new { gridJ1 = gridArrayJ1, gridJ2 = gridArrayJ2 });
})
.WithOpenApi();

// Route pour le tir
app.MapPost("/shoot", (GridService gridService, [FromBody] ShootRequest request) =>
{
    Console.WriteLine("shoot call");
    
    var shootResultJ1 = gridService.PlayerShoot(gridArrayJ2, request.X, request.Y);
    gridService.PrintGrid(gridArrayJ2, "gridJ2");
    // Logique pour vérifier si le jeu est terminé
    if (shootResultJ1.IsHit && gridService.IsGameFinished(gridArrayJ2)){
        return Results.Ok(new { message = "Le joueur 1 a gagné !" });
    }
    
    //code IA

    
    return Results.Ok(new { message = "Tir effectué.", hit = shootResultJ1.IsHit });
})
.WithOpenApi();

app.Run();

// Classe ShootRequest
public class ShootRequest
{
    public int X { get; set; } // Coordonnée X du tir
    public int Y { get; set; } // Coordonnée Y du tir
}
