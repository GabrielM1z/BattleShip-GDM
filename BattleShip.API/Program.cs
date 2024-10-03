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
    game.GameMode = "IA_2";

    game.PrintGame();

    return Results.Ok(new {Id = game.Id,IsGameFinished = game.IsGameFinished, GridJ1 = game.GridJ1, GridJ2 = game.GridJ2, MaskedGridJ1 = maskedJ1, MaskedGridJ2 = maskedJ2});
})
.WithOpenApi();




app.MapPost("/tour", (GridService gridService, [FromBody] ShootRequest request) =>
{
    Console.WriteLine("Tour du joueur et de l'IA");
    var gameresult = new GameShootResponse{};

    var playerShootResult = shoot(gridService, game, request);
    if (playerShootResult is Ok<GameShootResponse> okResult)
    {
        var result = okResult.Value;

        bool canShoot = result.shootResultJ1.CanShoot;
        bool isHit = result.shootResultJ1.IsHit;
        bool isGameFinished = result.game.IsGameFinished;

        if(!canShoot || isGameFinished){
            return playerShootResult;
        }
        gameresult.shootResultJ1 = result.shootResultJ1;
    }

    var (xIa, yIa) = manage_call_ia(game.GameMode, game.MaskedGridJ1);    

    var aiShootResult = shoot(gridService, game, new ShootRequest { X = xIa, Y = yIa, J = 2 });
    if (aiShootResult is Ok<GameShootResponse> okResultJ)
    {
        var result = okResultJ.Value;
        gameresult.game = result.game;
        gameresult.shootResultJ2 = result.shootResultJ2;
    }
    game.PrintGame();


    return Results.Ok(gameresult);
})
.WithOpenApi();



(int, int) manage_call_ia(string ia, bool?[][] grid)
{
    if(ia == "IA_1"){
        return GenerateValidIACoordinates_IA1(grid);
    }else if(ia == "IA_2"){
        return GenerateValidIACoordinates_IA2(grid);
    }else {
        return GenerateValidIACoordinates_IA1(grid);
    }
    
}

(int, int) GenerateValidIACoordinates_IA1(bool?[][] grid){
    Random random = new Random();
    int xIa, yIa;
    do
    {
        xIa = random.Next(grid.Length);
        yIa = random.Next(grid[0].Length);
    }
    while (grid[yIa][xIa] != null); 

    return (xIa, yIa);
}
(int, int) GenerateValidIACoordinates_IA2(bool?[][] grid){
    for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].Length; j++)
                {
                    if (grid[i][j]== null)
                    {
                        return (j,i);
                    }
                }
            }
    return (0,0);

}

static IResult shoot(GridService gridService, Game game, ShootRequest request)
{
    Console.WriteLine("shoot call");

    char[][] gridJoueur, gridAdverse;
    bool?[][] gridAdverseMasked;
    GameShootResponse shootResultJ1 = null, shootResultJ2 = null;

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
    bool gameFinished = false;

    if (shootResult.CanShoot)
    {
        gameFinished = shootResult.IsHit && gridService.IsGameFinished(gridAdverse,gridAdverseMasked);
        game.PrintGame();
    }

    if (request.J == 1)
    {
        shootResultJ1 = new GameShootResponse
        {
            game = new Game
            {
                IsGameFinished = gameFinished,
                GridJ1 = game.GridJ1,
                GridJ2 = game.GridJ2,
                MaskedGridJ1 = game.MaskedGridJ1,
                MaskedGridJ2 = game.MaskedGridJ2
            },
            shootResultJ1 = new ShootResult
            {
                CanShoot = shootResult.CanShoot,
                IsHit = shootResult.IsHit
            }
        };
    }
    // Préparer le résultat pour le joueur J2
    else if (request.J == 2)
    {
        shootResultJ2 = new GameShootResponse
        {
            game = new Game
            {
                IsGameFinished = gameFinished,
                GridJ1 = game.GridJ1,
                GridJ2 = game.GridJ2,
                MaskedGridJ1 = game.MaskedGridJ1,
                MaskedGridJ2 = game.MaskedGridJ2
            },
            shootResultJ2 = new ShootResult
            {
                CanShoot = shootResult.CanShoot,
                IsHit = shootResult.IsHit
            }
        };
    }

    // Retourner le résultat correct selon request.J
    return request.J == 1 ? Results.Ok(shootResultJ1) : Results.Ok(shootResultJ2);

    
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
