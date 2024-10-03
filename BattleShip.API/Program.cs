using System.Reflection.Metadata.Ecma335;
using BattleShip.API.Service;
using BattleShip.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;


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

bool isHunting = true;
(int x, int y)? lastHit = null;

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
    game.IsGameFinished = false;
    game.GridJ1 = gridJ1.GridArray;
    game.GridJ2 = gridJ2.GridArray;
    game.MaskedGridJ1 = maskedJ1;
    game.MaskedGridJ2 = maskedJ2;

    game.PrintGame();

    return Results.Ok(new {Id = game.Id,IsGameFinished = game.IsGameFinished, GridJ1 = game.GridJ1, GridJ2 = game.GridJ2, MaskedGridJ1 = maskedJ1, MaskedGridJ2 = maskedJ2});
})
.WithOpenApi();




app.MapPost("/tour", (GridService gridService, [FromBody] ShootRequest request) =>
{
    Console.WriteLine("Tour du joueur et de l'IA");

    var playerShootResult = shoot(gridService, game, request);

    if (playerShootResult is Ok<GameShootResponse> okResult)
    {
        var result = okResult.Value;

        bool canShoot = result.shootResult.CanShoot;
        bool isHit = result.shootResult.IsHit;
        bool isGameFinished = result.game.IsGameFinished;

        Console.WriteLine($"Can Shoot: {canShoot}, Is Hit: {isHit}, Is Game Finished: {isGameFinished}");
        if(!canShoot || isGameFinished){
            return playerShootResult;
        }

    }

    var (xIa, yIa) = GenerateValidIACoordinates(game.MaskedGridJ1);    
    //var (xIa, yIa) = GenerateValidIACoordinates(game.MaskedGridJ1, isHunting, lastHit);

    var aiShootResult = shoot(gridService, game, new ShootRequest { X = xIa, Y = yIa, J = 2 });

    game.PrintGame();

    return aiShootResult;
})
.WithOpenApi();





// Générer les coordonnées de tir de l'IA (fonction externe)
(int, int) GenerateValidIACoordinates(bool?[][] grid)
{
    Random random = new Random();
    int xIa, yIa;
    
    do
    {
        xIa = random.Next(grid.Length);  // Génère une coordonnée X
        yIa = random.Next(grid[0].Length);  // Génère une coordonnée Y
    }
    while (grid[yIa][xIa] != null); // Évite les tirs déjà effectués

    return (xIa, yIa);
}
/*
(int, int) GenerateValidIACoordinates(bool?[][] grid, bool isHunting, (int x, int y)? lastHit = null)
{
    Random random = new Random();
    int xIa = -1, yIa = -1;
    if (lastHit != null){
        var (lastX, lastY) = lastHit.Value;
        if(grid[lastY][lastX] == true){
            isHunting = true;
        }
    }

    // Phase de ciblage : si l'IA a touché un bateau précédemment, elle cible autour
    if (!isHunting && lastHit != null)
    {
        var (lastX, lastY) = lastHit.Value;
        var potentialTargets = new List<(int x, int y)>
        {
            (lastX - 1, lastY), // Case au-dessus
            (lastX + 1, lastY), // Case en-dessous
            (lastX, lastY - 1), // Case à gauche
            (lastX, lastY + 1)  // Case à droite
        };
        
        // Filtrer les cases valides (dans la grille et non déjà visées)
        potentialTargets = potentialTargets
            .Where(coord => coord.x >= 0 && coord.x < grid.Length && coord.y >= 0 && coord.y < grid[0].Length)
            .Where(coord => grid[coord.y][coord.x] == null) // Non encore visée
            .ToList();

        // Si on trouve des cibles valides, on en sélectionne une au hasard
        if (potentialTargets.Count > 0)
        {
            var target = potentialTargets[random.Next(potentialTargets.Count)];
            return target;
        }
    }

    // Phase de chasse : si l'IA n'a pas encore trouvé de bateau ou a coulé le bateau, elle chasse
    do
    {
        // Utiliser un motif en damier pour maximiser les chances
        xIa = random.Next(grid.Length);
        yIa = random.Next(grid[0].Length);
        
        // Motif en damier : tire seulement sur les cases où (xIa + yIa) est pair
    } while (grid[yIa][xIa] != null || (xIa + yIa) % 2 != 0);

    
    return (xIa, yIa);
}
*/

static IResult shoot(GridService gridService, Game game, ShootRequest request)
{
    Console.WriteLine("shoot call");

    char[][] gridJoueur, gridAdverse;
    bool?[][] gridAdverseMasked;

    switch (request.J)
    {
        case 1:
            gridJoueur = game.GridJ1;
            gridAdverse = game.GridJ2;
            gridAdverseMasked = game.MaskedGridJ2;
            break;
        case 2:
            gridJoueur = game.GridJ2; 
            gridAdverse = game.GridJ1;
            gridAdverseMasked = game.MaskedGridJ1;
            break;
        default:
            return Results.BadRequest(new { message = "Joueur invalide." });
    }

    var shootResult = gridService.PlayerShoot(gridAdverse, gridAdverseMasked, request.X, request.Y);

    if (!shootResult.CanShoot)
    {
        return Results.Ok(new GameShootResponse
        {
            game = new Game
            {
                IsGameFinished = false,
                GridJ1 = game.GridJ1,
                GridJ2 = game.GridJ2,
                MaskedGridJ1 = game.MaskedGridJ1,
                MaskedGridJ2 = game.MaskedGridJ2
            },
            shootResult = new ShootResult { CanShoot = false, IsHit = false }
        });
    }

    bool gameFinished = shootResult.IsHit && gridService.IsGameFinished(gridAdverse);

    //game.MaskedGridJ1 = gridService.CreateMaskedGrid(game.GridJ1);
    //game.MaskedGridJ2 = gridService.CreateMaskedGrid(game.GridJ2);

    game.PrintGame();

    return Results.Ok(new GameShootResponse
    {
        game = new Game
        {
            IsGameFinished = gameFinished,
            GridJ1 = game.GridJ1,
            GridJ2 = game.GridJ2,
            MaskedGridJ1 = game.MaskedGridJ1,
            MaskedGridJ2 = game.MaskedGridJ2
        },
        shootResult = new ShootResult
        {
            CanShoot = shootResult.CanShoot,
            IsHit = shootResult.IsHit
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
