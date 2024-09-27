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
    bool?[,] maskgridJ2 = gridService.MaskedGrid(gridJ2);
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
    char[][] gridAdverse = null;
    if (request.J == 1){
        gridAdverse = gridArrayJ2;
    }else{
        gridAdverse = gridArrayJ1;
    }

    var shootResult = gridService.PlayerShoot(gridAdverse, request.X, request.Y);
    if (shootResult.CanShoot == false){
        return Results.Ok(new { message = "Tir impossible.", shoot = shootResult.CanShoot });
    }
    gridService.PrintGrid(gridAdverse, "Grille jouée");
    bool gameFinished = false;
    if (shootResult.IsHit && gridService.IsGameFinished(gridAdverse)){
        gameFinished = true;
    }
    
    return Results.Ok(new { message = "Tir effectué.", shoot = shootResult.CanShoot, hit = shootResult.IsHit, isGameFinished = gameFinished });
})
.WithOpenApi();

app.MapPost("/tour", (GridService gridService, [FromBody] ShootRequest request) =>
{
    Console.WriteLine("Tour du joueur et de l'IA");

    // Tir du joueur
    char[][] gridAdverseJoueur = (request.J == 1) ? gridArrayJ2 : gridArrayJ1; // Si le joueur est J1, il tire sur la grille de J2
    var shootResultJoueur = gridService.PlayerShoot(gridAdverseJoueur, request.X, request.Y);
    if (shootResultJoueur.CanShoot == false){
        return Results.Ok(new { message = "Tir impossible.", shoot = shootResultJoueur.CanShoot });
    }
    gridService.PrintGrid(gridAdverseJoueur, "Grille après tir du joueur");
    bool gameFinishedJoueur = false;

    // Vérifier si le joueur a gagné
    if (shootResultJoueur.IsHit && gridService.IsGameFinished(gridAdverseJoueur))
    {
        gameFinishedJoueur = true;
        return Results.Ok(new { message = "Tir du joueur effectué.", hit = shootResultJoueur.IsHit, isGameFinished = gameFinishedJoueur, winner = "Joueur" });
    }

    // Tour de l'IA
    Random random = new Random();
    int xIa, yIa;
    char[][] gridAdverseIA = gridArrayJ1; // L'IA tire toujours sur la grille du joueur 1

    do
    {
        xIa = random.Next(gridAdverseIA.Length);  // Génère une coordonnée X
        yIa = random.Next(gridAdverseIA[0].Length);  // Génère une coordonnée Y
    } while (gridAdverseIA[xIa][yIa] == 'X' || gridAdverseIA[xIa][yIa] == 'O'); // Évite les tirs déjà effectués

    var shootResultIA = gridService.PlayerShoot(gridAdverseIA, xIa, yIa);
    gridService.PrintGrid(gridAdverseIA, "Grille après tir de l'IA");
    bool gameFinishedIA = false;

    // Vérifier si l'IA a gagné
    if (shootResultIA.IsHit && gridService.IsGameFinished(gridAdverseIA))
    {
        gameFinishedIA = true;
        return Results.Ok(new { message = "Tir de l'IA effectué.", hit = shootResultIA.IsHit, isGameFinished = gameFinishedIA, winner = "IA" });
    }

    return Results.Ok(new
    {
        message = "Tirs effectués.",
        joueur = new { hit = shootResultJoueur.IsHit, isGameFinished = gameFinishedJoueur },
        ia = new { hit = shootResultIA.IsHit, isGameFinished = gameFinishedIA }
    });
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
