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
using Microsoft.EntityFrameworkCore;



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

// Ajout du DbContext pour gérer la base de données
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajoute le service GridService au conteneur DI
builder.Services.AddSingleton<GridService>();
builder.Services.AddSingleton<AI>();
builder.Services.AddSingleton<Game>();
builder.Services.AddSingleton<GameHistory>();
builder.Services.AddScoped<IValidator<SetupRequest>, SetupRequestValidator>();
builder.Services.AddScoped<IValidator<PlaceRequest>, PlaceRequestValidator>();
builder.Services.AddScoped<IValidator<Boat>, BoatValidator>();
builder.Services.AddScoped<IValidator<ShootRequest>, ShootRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate(); // Appliquer les migrations
}

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

app.MapPost("/setup", (GridService gridService, Game game, GameHistory gameHistory, [FromBody] SetupRequest request, IValidator<SetupRequest> validator) =>
{
    Console.WriteLine("/setup call");

    // Valider la requête
    var validationResult = validator.Validate(request);

    // Si la validation échoue, renvoyer une erreur
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    User user = new User(request.User);

    var gameId = Guid.NewGuid();


    bool pve = request.LevelDifficulty[0] == '1'; // Premier caractère : PVE = true, sinon PVP
    int CodeLvl = int.Parse(request.LevelDifficulty[1].ToString()); // Deuxième caractère : niveau IA et taille de la grille

    int gridSize = 8; // Valeur par défaut
    int aiLevel = 1; // Valeur par défaut (niveau de l'IA)

    // Logique de détermination de la grille et du niveau de l'IA en fonction du code
    switch (CodeLvl)
    {
        case 0: 
            gridSize = 8; // Grille de taille 8
            aiLevel = pve ? 1 : 0; // Niveau IA 1
            break;
        case 1:
            gridSize = pve ? 8 : 10; // Grille de taille 8
            aiLevel = pve ? 2 : 0; // Niveau IA 2
            break;
        case 2:
            gridSize = pve ? 10 : 12; // Grille de taille 10
            aiLevel = pve ? 3 : 0; // Niveau IA 3
            break;
        case 3:
            gridSize = pve ? 10 : 12; // Grille de taille 10
            aiLevel = pve ? 4 : 0; // Niveau IA 4
            break;
        case 4:
            gridSize = 12; // Grille de taille 12
            aiLevel = pve ? 5 : 0; // Niveau IA 4
            break;
        default:
            throw new ArgumentException("Code de niveau non valide.");
    }
    



    Grid gridJ1 = gridService.CreateGrid(gridSize);
    Grid gridJ2 = gridService.CreateGrid(gridSize);

    var maskedJ1 = gridService.CreateMaskedGrid(gridJ1.GridArray);
    var maskedJ2 = gridService.CreateMaskedGrid(gridJ2.GridArray);

    game.Id = gameId;
    game.user = user;
    game.IsGameFinished = false;
    game.GridJ1 = gridJ1.GridArray;
    game.GridJ2 = gridJ2.GridArray;
    game.MaskedGridJ1 = maskedJ1;
    game.MaskedGridJ2 = maskedJ2;
    game.GameMode = aiLevel;
    game.fleetJ1 = new Fleet(true);
    game.fleetJ2 = new Fleet(true);

    gameHistory = new GameHistory();

    return Results.Ok(game);
}).WithOpenApi();



app.MapPost("/start", (GridService gridService, Game game, GameHistory gameHistory, [FromBody] PlaceRequest request, IValidator<PlaceRequest> validator) =>
{
    Console.WriteLine("/start call");

    
    // Valider la requête
    var validationResult = validator.Validate(request);

    // Si la validation échoue, renvoyer une erreur
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    Fleet boatsJ1 = new Fleet(true);
    if (request.Boats != null)
    {
        Console.WriteLine($"request.Boats != null");
        foreach (Boat boat in request.Boats)
        {
            Console.WriteLine($"Boat boat in request.Boats");
            boatsJ1.SetBoatPosition(boat.Id,boat.X,boat.Y,boat.Horizontal);
            Console.WriteLine("Ajout");
        }
    }

    Fleet boatsJ2 = new Fleet(true); 

    gridService.PlaceBoat(game.GridJ1, boatsJ1.GetBoats());
    gridService.PlaceBoat(game.GridJ2, boatsJ2.GetBoats());


    game.fleetJ1 = boatsJ1;
    game.fleetJ2 = boatsJ2;

    game.PrintGame();

    gameHistory.InitSave(game);

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




app.MapPost("/tour", async (AppDbContext dbContext, GridService gridService, Game game, AI AI, GameHistory gameHistory, [FromBody] ShootRequest request, IValidator<ShootRequest> validator) =>
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

    if (canShoot && !isGameFinished)
    {
        game.user.NbCoup++;
    }
    else
    {
        if (isGameFinished)
        {
            // Récupérer l'utilisateur
            var userRecup = await dbContext.Users.FirstOrDefaultAsync(u => u.Name == game.user.Name);

            // Vérifie si l'utilisateur existe
            if (userRecup != null)
            {
                // Si le nombre de coups du jeu est supérieur à celui de l'utilisateur récupéré, on met à jour
                if (game.user.NbCoup < userRecup.NbCoup)
                {
                    userRecup.NbCoup = game.user.NbCoup; // Met à jour l'utilisateur récupéré
                }
            }
            else
            {
                // Si l'utilisateur n'existe pas, on le crée
                userRecup = game.user;
                dbContext.Users.Add(userRecup);
            }

            // Incrémente le nombre de victoires
            userRecup.NbVictoire++;

            // Enregistre ou met à jour l'utilisateur dans la base de données
            dbContext.Users.Update(userRecup);

            // Sauvegarde les changements dans la base de données
            await dbContext.SaveChangesAsync();
        }
        return playerShootResult; // Si le joueur ne peut pas tirer ou si la partie est finie
    }
    
    gameresult.shootResultJ1 = result.shootResultJ1;
    

    var (xIa, yIa) = AI.manage_call_ia(game.GameMode, game.MaskedGridJ1, game.fleetJ1);    
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
    
    gameHistory.SaveState(game);
    //game.SetGame(gameHistory.GetCurrentState());

    return Results.Ok(gameresult);
})
.WithOpenApi();

app.MapPost("/shoot", (GridService gridService, Game game, [FromBody] ShootRequest request, IValidator<ShootRequest> validator) => 
{
    return shoot(gridService, game, request, validator);
})
.WithOpenApi();


app.MapGet("/undo", (GridService gridService, Game game, GameHistory gameHistory) => 
{
    Console.WriteLine($"/undo");
    GameStateHisto previousState = gameHistory.Undo();
    if (previousState != null)
    {
        game.SetGame(previousState);
        game.user.NbCoup++; // Incrémenter le nombre de coups joués
    }
    game.PrintGame();
    return Results.Ok(game);
})
.WithOpenApi();

app.MapGet("/getLeaderBoard", async (AppDbContext dbContext) =>
{
    // Récupérer tous les utilisateurs
    var users = await dbContext.Users.ToListAsync();

    // Créer les dictionnaires pour le classement
    var coupCountMap = users.ToDictionary(u => u.Name, u => u.NbCoup);
    var victoryCountMap = users.ToDictionary(u => u.Name, u => u.NbVictoire);

    // Trier les dictionnaires par ordre croissant
    var sortedCoupCount = coupCountMap.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
    var sortedVictoryCount = victoryCountMap.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

    // Créer l'objet de réponse
    var response = new LeaderBoardResult
    {
        UserCoupCount = sortedCoupCount,
        UserVictoryCount = sortedVictoryCount
    };

    return Results.Ok(response);
})
.WithOpenApi();

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


