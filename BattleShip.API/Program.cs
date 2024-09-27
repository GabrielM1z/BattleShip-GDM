using System.Reflection.Metadata.Ecma335;
using BattleShip.API.Service;
using BattleShip.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5148") // Adresse du front-end Blazor WebAssembly
              .AllowAnyMethod() // Autoriser toutes les méthodes HTTP (GET, POST, etc.)
              .AllowAnyHeader() // Autoriser tous les en-têtes
              .AllowCredentials(); // Si tu utilises des cookies ou des identifiants
    });
});

// Ajoute le service GridService au conteneur DI
builder.Services.AddSingleton<GridService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

char[][] gridArrayJ1 = null;
char[][] gridArrayJ2 = null;
bool?[,] maskgridJ2 = null;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");

app.MapGet("/", () => "Hello World")
.WithOpenApi();

// Route pour start la game
app.MapGet("/start", (GridService gridService) =>
{
    var gameId = Guid.NewGuid();

    Grid gridJ1 = gridService.CreateGrid();
    gridArrayJ1 = gridService.GetGridArray(gridJ1);


    gridService.PrintGrid(gridArrayJ1, "gridJ1");

    Grid gridJ2 = gridService.CreateGrid();
    gridArrayJ2 = gridService.GetGridArray(gridJ2);
    maskgridJ2 = gridService.MaskedGrid(gridJ2);
    bool?[][] gridArray = gridService.GetBoolGridArray(maskgridJ2);

    gridService.PrintGrid(gridArrayJ2, "gridJ2");

    var game = new Game
    {
        Id = gameId,
        GridJ1 = gridArrayJ1,
        GridJ2 = gridArrayJ2,
        MaskedGridJ2 = gridArray,
    };

    return Results.Ok(game);
})
.WithOpenApi();

// Route pour le tir
app.MapPost("/shoot", (GridService gridService, [FromBody] ShootRequest request) =>
{
    Console.WriteLine("shoot call");
    
    var shootResultJ1 = gridService.PlayerShoot(gridArrayJ2, request.X, request.Y);
    gridService.PrintGrid(gridArrayJ2, "Grille jouée");
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
    public int J { get; set; } // Joueur qui tir
}
