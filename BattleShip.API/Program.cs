var builder = WebApplication.CreateBuilder(args);

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

/// Route Hello World
app.MapGet("/", () => "Hello World")
.WithOpenApi();


/// Route creation 
app.MapGet("/createGrid", () => {
    
    // Générer deux matrices 10x10
    int[,] matrix1 = GenerateMatrix(10, 10);
    int[,] matrix2 = GenerateMatrix(10, 10);

    var result = new {
        Matrix1 = matrix1,
        Matrix2 = matrix2
    };

    return Results.Json(result);
})
.WithOpenApi();

app.Run();

