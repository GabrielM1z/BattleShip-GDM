using System.Reflection.Metadata.Ecma335;
using BattleShip.API.Service;
using BattleShip.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;


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
var game = new Game{};

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
    Grid gridJ2 = gridService.CreateGrid();

    var maskedJ1 = gridService.CreateMaskedGrid(gridJ1.GridArray);
    var maskedJ2 = gridService.CreateMaskedGrid(gridJ2.GridArray);

    game.Id = gameId;
    game.GridJ1 = gridJ1.GridArray;
    game.GridJ2 = gridJ2.GridArray;
    game.MaskedGridJ1 = maskedJ1;
    game.MaskedGridJ2 = maskedJ2;

    game.PrintGame();

    return Results.Ok(new {message = game.Id, GridJ1 = game.GridJ1, GridJ2 = game.GridJ2, MaskedGridJ1 = maskedJ1, MaskedGridJ2 = maskedJ2});
})
.WithOpenApi();





app.MapPost("/tour", (GridService gridService, [FromBody] ShootRequest request) =>
{
    Console.WriteLine("Tour du joueur et de l'IA");

    // Vérifie si le joueur a bien sélectionné une grille adverse
    char[][] gridJoueur = null;
    char[][] gridAdverse = null;
    if (request.J == 1)
    {
        gridJoueur = game.GridJ1; 
        gridAdverse = game.GridJ2; // Le joueur 1 tire sur la grille du joueur 2
    }
    else if (request.J == 2)
    {
        gridAdverse = game.GridJ1; // Le joueur 2 tire sur la grille du joueur 1
        gridJoueur = game.GridJ2;
    }
    else
    {
        return Results.BadRequest(new { message = "Joueur invalide." });
    }

    // Effectue le tir
    var shootResult = gridService.PlayerShoot(gridAdverse, request.X, request.Y);
    
    
    // Si le tir est impossible, on retourne un message approprié
    if (!shootResult.CanShoot)
    {
        return Results.Ok(new { message = "Tir impossible.", shoot = shootResult.CanShoot });
    }

    
    // Vérifier si le joueur a gagné
    if (shootResult.IsHit && gridService.IsGameFinished(gridAdverse))
    {
        return Results.Ok(new
        {
            message = "Tir du joueur effectué.",
            hit = shootResult.IsHit,
            isGameFinished = true,
            winner = "Joueur"
        });
    }

    // 2. Tir de l'IA
    var (xIa, yIa) = GenerateValidIACoordinates(gridJoueur);

    var shootResultIA = gridService.PlayerShoot(gridJoueur, xIa, yIa);

    // Vérifier si l'IA a gagné
    if (shootResultIA.IsHit && gridService.IsGameFinished(gridJoueur))
    {
        return Results.Ok(new
        {
            message = "Tir de l'IA effectué.",
            hit = shootResultIA.IsHit,
            isGameFinished = true,
            winner = "IA"
        });
    }

    game.MaskedGridJ1 = gridService.CreateMaskedGrid(game.GridJ1);
    game.MaskedGridJ2 = gridService.CreateMaskedGrid(game.GridJ2);
    
    game.PrintGame();
    // 3. Retourner les résultats des deux tirs (joueur et IA)
    return Results.Ok(new
    {
        message = "Tirs effectués.",
        joueur = new
        {
            hit = shootResult.IsHit,
            isGameFinished = false,
            x = request.X,
            y = request.Y
        },
        ia = new
        {
            hit = shootResultIA.IsHit,
            isGameFinished = false,
            x = xIa,
            y = yIa
        },
        GridJ1 = game.GridJ1,
        GridJ2 = game.GridJ2,
        MaskedGridJ1 = game.MaskedGridJ1,
        MaskedGridJ2 = game.MaskedGridJ2
    });
})
.WithOpenApi();




// Générer les coordonnées de tir de l'IA (fonction externe)
(int, int) GenerateValidIACoordinates(char[][] grid)
{
    Random random = new Random();
    int xIa, yIa;

    do
    {
        xIa = random.Next(grid.Length);  // Génère une coordonnée X
        yIa = random.Next(grid[0].Length);  // Génère une coordonnée Y
    }
    while (grid[xIa][yIa] == 'X' || grid[xIa][yIa] == 'O'); // Évite les tirs déjà effectués

    return (xIa, yIa);
}

// Définir la fonction Toto sans modificateur d'accès
static IResult shoot(GridService gridService, Game game,ShootRequest request)
{
    Console.WriteLine("shoot call");

    // Vérifie si le joueur a bien sélectionné une grille adverse
    char[][] gridJoueur = null;
    char[][] gridAdverse = null;
    if (request.J == 1)
    {
        gridJoueur = game.GridJ1; 
        gridAdverse = game.GridJ2; // Le joueur 1 tire sur la grille du joueur 2
    }
    else if (request.J == 2)
    {
        gridAdverse = game.GridJ1; // Le joueur 2 tire sur la grille du joueur 1
        gridJoueur = game.GridJ2;
    }
    else
    {
        return Results.BadRequest(new { message = "Joueur invalide." });
    }

    // Effectue le tir
    var shootResult = gridService.PlayerShoot(gridAdverse, request.X, request.Y);
    
    // Si le tir est impossible, on retourne un message approprié
    if (!shootResult.CanShoot)
    {
        return Results.Ok(new { message = "Tir impossible.", shoot = shootResult.CanShoot });
    }

    // Vérifie si le jeu est terminé
    bool gameFinished = false;
    if (shootResult.IsHit && gridService.IsGameFinished(gridAdverse))
    {
        gameFinished = true;
    }

    game.MaskedGridJ1 = gridService.CreateMaskedGrid(game.GridJ1);
    game.MaskedGridJ2 = gridService.CreateMaskedGrid(game.GridJ2);

    game.PrintGame();
    
    // Retourne le résultat du tir
    return Results.Ok(new 
    { 
        Game = new {
            message = "Tir effectué.", 
            isGameFinished = gameFinished,
            GridJ1 = game.GridJ1,
            GridJ2 = game.GridJ2,
            MaskedGridJ1 = game.MaskedGridJ1,
            MaskedGridJ2 = game.MaskedGridJ2
        },
        ShootResult = new{
            shoot = shootResult.CanShoot,
            hit = shootResult.IsHit
            
        }
    });
}

// Exemple d'utilisation de la fonction Toto dans une route
app.MapPost("/shoot", (GridService gridService, [FromBody] ShootRequest request) => 
{
    return shoot(gridService, game, request);
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

