using System.Reflection.Metadata.Ecma335;
using BattleShip.API.Service;
using BattleShip.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Diagnostics;
using FluentValidation;


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
builder.Services.AddSingleton<Game>();
builder.Services.AddScoped<IValidator<ShootRequest>, ShootRequestValidator>();


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
app.UseCors("AllowBlazorClient");

app.MapGet("/", () => "Hello World")
.WithOpenApi();

app.MapPost("/setup", (GridService gridService, Game game, [FromBody] LevelRequest request) =>
{
    Console.WriteLine("/setup call");
    var gameId = Guid.NewGuid();


    bool pve = request.LevelDifficulty[0] == '1'; // Premier caractère : PVE = true, sinon PVP
    int aiCode = int.Parse(request.LevelDifficulty[1].ToString()); // Deuxième caractère : niveau IA et taille de la grille

    int gridSize = 8; // Valeur par défaut
    int aiLevel = 1; // Valeur par défaut (niveau de l'IA)

    // Logique de détermination de la grille et du niveau de l'IA en fonction du code
    switch (aiCode)
    {
        case 0: 
            gridSize = 8; // Grille de taille 8
            aiLevel = 1; // Niveau IA 1
            break;
        case 1:
            gridSize = 8; // Grille de taille 8
            aiLevel = 2; // Niveau IA 2
            break;
        case 2:
            gridSize = 10; // Grille de taille 10
            aiLevel = 3; // Niveau IA 3
            break;
        case 3:
            gridSize = 10; // Grille de taille 10
            aiLevel = 4; // Niveau IA 4
            break;
        case 4:
            gridSize = 12; // Grille de taille 12
            aiLevel = 4; // Niveau IA 4
            break;
        default:
            throw new ArgumentException("Code de niveau IA non valide.");
    }
    






    Fleet fleet = new Fleet(true);

    Grid gridJ1 = gridService.CreateGrid(gridSize);
    Grid gridJ2 = gridService.CreateGrid(gridSize);

    var maskedJ1 = gridService.CreateMaskedGrid(gridJ1.GridArray);
    var maskedJ2 = gridService.CreateMaskedGrid(gridJ2.GridArray);

    game.Id = gameId;
    game.IsGameFinished = false;
    game.GridJ1 = gridJ1.GridArray;
    game.GridJ2 = gridJ2.GridArray;
    game.MaskedGridJ1 = maskedJ1;
    game.MaskedGridJ2 = maskedJ2;
    game.GameMode = aiLevel;

    var boats = fleet.GetBoatsWithoutIsAlive();
    return Results.Ok(boats);
}).WithOpenApi();


app.MapPost("/start", (GridService gridService, Game game, [FromBody] PlaceRequest requeste) =>
{
    Console.WriteLine("/start call");

    

    Fleet boatsJ1 = new Fleet(true);
    if(requeste.Boats.Count > 0){
        boatsJ1.Boats = requeste.Boats;
    }
    Fleet boatsJ2 = new Fleet(true);

    gridService.PlaceBoat(game.GridJ1, boatsJ1.GetBoats());
    gridService.PlaceBoat(game.GridJ2, boatsJ2.GetBoats());


    
    game.fleetJ1 = boatsJ1;
    game.fleetJ2 = boatsJ2;

    game.PrintGame();

    return Results.Ok(new
    {
        Id = game.Id,
        IsGameFinished = game.IsGameFinished,
        GridJ1 = game.GridJ1,
        //GridJ2 = game.GridJ2,
        MaskedGridJ1 = game.MaskedGridJ1,
        MaskedGridJ2 = game.MaskedGridJ2
    });
})
.WithOpenApi();




app.MapPost("/tour", (GridService gridService, Game game, [FromBody] ShootRequest request, IValidator<ShootRequest> validator) =>
{
    Console.WriteLine("\n\n\nTour du joueur");
    var gameresult = new GameShootResponse{};

    var playerShootResult = shoot(gridService, game, request, validator);
    if (playerShootResult is not Ok<GameShootResponse> okResult)
    {
        return playerShootResult; // Retourner l'erreur si ce n'est pas un résultat valide
    }

    var result = okResult.Value;

    bool canShoot = result.shootResultJ1.CanShoot;
    bool isHit = result.shootResultJ1.IsHit;
    bool isGameFinished = result.game.IsGameFinished;

    if(!canShoot || isGameFinished){
        return playerShootResult;
    }
    gameresult.shootResultJ1 = result.shootResultJ1;
    

    var (xIa, yIa) = manage_call_ia(game.GameMode, game.MaskedGridJ1, game.fleetJ1);    
    Console.WriteLine($"Tour de l'IA:{game.GameMode}");
    var aiShootResult = shoot(gridService, game, new ShootRequest { X = xIa, Y = yIa, J = 2 }, validator);
    if (aiShootResult is not Ok<GameShootResponse> okResultJ)
    {
        return aiShootResult;
    }
    
    result = okResultJ.Value;
    gameresult.game = result.game;
    gameresult.shootResultJ2 = result.shootResultJ2;
    game.PrintGame();


    return Results.Ok(gameresult);
})
.WithOpenApi();

app.MapPost("/shoot", (GridService gridService, Game game, [FromBody] ShootRequest request, IValidator<ShootRequest> validator) => 
{
    return shoot(gridService, game, request, validator);
})
.WithOpenApi();

/*
app.MapGet("/...", () =>
{
    //histo
}).WithOpenApi();
*/

(int, int) manage_call_ia(int ia, bool?[][] grid, Fleet fleet)
{
    if(ia == 1){
        return GenerateValidIACoordinates_IA1(grid);
    }else if(ia == 2){
        return GenerateValidIACoordinates_IA2(grid);
    }else if(ia == 3){
        return GenerateValidIACoordinates_IA3(grid);
    }else if(ia == 4){
        return GenerateValidIACoordinates_IA4(grid, fleet);
    }else{
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
(int, int) GenerateValidIACoordinates_IA3(bool?[][] grid){
    for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].Length; j++)
                {
                    if (grid[i][j] == true && CanShootAround(grid, i, j))
                    {
                        if (i > 0 && grid[i - 1][j] == null) // Vérifie la case au-dessus
                            return (j,i-1);
                        
                        if (i < grid.Length - 1 && grid[i + 1][j] == null) // Vérifie la case en-dessous
                            return (j,i+1);

                        if (j > 0 && grid[i][j - 1] == null) // Vérifie la case à gauche
                            return (j-1,i);

                        if (j < grid[i].Length - 1 && grid[i][j + 1] == null) // Vérifie la case à droite
                            return (j+1,i);
                        //return (j,i);
                    }
                }
            }
    return GenerateValidIACoordinates_IA1(grid);

}
(int, int) GenerateValidIACoordinates_IA4(bool?[][] grid, Fleet fleet){
    bool areAllBoatsSunk = true;  //il y a un bateau touché mais non coulé
    areAllBoatsSunk = CheckSinkBoat(grid, fleet);
    Console.WriteLine($"All boats sink {areAllBoatsSunk}");

    for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].Length; j++)
                {
                    if (!areAllBoatsSunk && grid[i][j] == true && CanShootAround(grid, i, j))
                    {
                        Console.WriteLine($"--- ICI {areAllBoatsSunk}");
                        if (i > 0 && grid[i - 1][j] == null) // Vérifie la case au-dessus
                            return (j,i-1);
                        
                        if (i < grid.Length - 1 && grid[i + 1][j] == null) // Vérifie la case en-dessous
                            return (j,i+1);

                        if (j > 0 && grid[i][j - 1] == null) // Vérifie la case à gauche
                            return (j-1,i);

                        if (j < grid[i].Length - 1 && grid[i][j + 1] == null) // Vérifie la case à droite
                            return (j+1,i);
                    }
                }
            }
    return GenerateValidIACoordinates_IA1(grid);
}

bool CheckSinkBoat(bool?[][] grid, Fleet fleet)
{
    int totalSunkBoatSize = fleet.Boats
        .Where(boat => !boat.IsAlive)   // Filtrer les bateaux coulés
        .Sum(boat => boat.Size);        // Additionner la taille des bateaux coulés

    int trueCountInGrid = grid.Sum(row => row.Count(cell => cell == true));

    bool areAllBoatsSunk = (totalSunkBoatSize == trueCountInGrid);
    return areAllBoatsSunk;
}


bool CanShootAround(bool?[][] grid, int i, int j)
{
    Console.WriteLine($"canshootaround {i}{j}");
    int nb = 0;
    // Vérifier les limites pour éviter les accès hors des bords de la grille
    if (i > 0 && grid[i - 1][j] != null) // Vérifie la case au-dessus
        nb++;
    
    if (i < grid.Length - 1 && grid[i + 1][j] != null) // Vérifie la case en-dessous
        nb++;

    if (j > 0 && grid[i][j - 1] != null) // Vérifie la case à gauche
        nb++;

    if (j < grid[i].Length - 1 && grid[i][j + 1] != null) // Vérifie la case à droite
        nb++;

    // Si toutes les cases adjacentes sont null, retourner true
    if (nb == 4){
        return false;
    }else{
        return true;
    }
}


static IResult shoot(GridService gridService, Game game, ShootRequest request, IValidator<ShootRequest> validator)
{
     
    
    // Valider la requête
    var validationResult = validator.Validate(request);

    // Si la validation échoue, renvoyer une erreur
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    // Si c'est valide, exécuter la logique du tir
    char[][] gridJoueur = Array.Empty<char[]>();
    char[][] gridAdverse = Array.Empty<char[]>();
    bool?[][] gridAdverseMasked = Array.Empty<bool?[]>();
    Fleet fleetAdverse = new Fleet();
    GameShootResponse shootResultJ1 = null, shootResultJ2 = null;

    switch (request.J)
    {
        case 1:
            gridAdverse = game.GridJ2;
            gridAdverseMasked = game.MaskedGridJ2;
            fleetAdverse = game.fleetJ2;
            break;
        case 2:
            gridAdverse = game.GridJ1;
            gridAdverseMasked = game.MaskedGridJ1;
            fleetAdverse = game.fleetJ1;
            break;
    }

    var shootResult = gridService.PlayerShoot(gridAdverse, gridAdverseMasked, request.X, request.Y);
    bool gameFinished = false;

    if (shootResult.CanShoot)
    {
        gameFinished = shootResult.IsHit && gridService.IsGameFinished(gridAdverse,gridAdverseMasked);
    }

    fleetAdverse.UpdateBoats(gridAdverse,gridAdverseMasked);

    char lettre_shoot = gridAdverse[request.Y][request.X];
    String mess = fleetAdverse.IsShootSink(lettre_shoot);

    var SendGame = new Game
    {
        IsGameFinished = gameFinished,
        //GridJ1 = game.GridJ1,
        //GridJ2 = game.GridJ2,
        MaskedGridJ1 = game.MaskedGridJ1,
        MaskedGridJ2 = game.MaskedGridJ2,
        fleetJ1 = game.fleetJ1,
        fleetJ2 = game.fleetJ2
    };

    if (request.J == 1)
    {
        SendGame.GridJ1 = game.GridJ1;
        shootResultJ1 = new GameShootResponse
        {
            game = SendGame,
            shootResultJ1 = new ShootResult
            {
                CanShoot = shootResult.CanShoot,
                IsHit = shootResult.IsHit,
                Message = string.IsNullOrEmpty(mess) ? shootResult.Message : mess
            }
        };
    }
    else if (request.J == 2)
    {
        SendGame.GridJ1 = game.GridJ1;
        shootResultJ2 = new GameShootResponse
        {
            game = SendGame,
            shootResultJ2 = new ShootResult
            {
                CanShoot = shootResult.CanShoot,
                IsHit = shootResult.IsHit,
                Message = string.IsNullOrEmpty(mess) ? shootResult.Message : mess
            }
        };
    }

    // Retourner le résultat correct selon request.J
    return request.J == 1 ? Results.Ok(shootResultJ1) : Results.Ok(shootResultJ2);   
}

app.Run();

public class ShootRequest
{
    public int X { get; set; } // Coordonnée X du tir
    public int Y { get; set; } // Coordonnée Y du tir
    public int J { get; set; } // Joueur qui tir
}


