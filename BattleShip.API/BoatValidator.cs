using BattleShip.Models;
using FluentValidation;

public class BoatValidator : AbstractValidator<Boat>
{
    public BoatValidator(Game game)
    {
        var gridSize = game.GridJ1.Length;

        // Validation pour la taille du bateau
        RuleFor(b => b.Size)
            .GreaterThan(0).WithMessage("La taille du bateau doit être positive.")
            .LessThanOrEqualTo(5).WithMessage("La taille du bateau ne doit pas dépasser 5 cases.");

        // Validation pour la coordonnée X
        RuleFor(b => b.X)
            .GreaterThanOrEqualTo(0).WithMessage("La coordonnée X doit être positive.")
            .LessThan(gridSize).WithMessage($"La coordonnée X doit être inférieure à {gridSize}.");

        // Validation pour la coordonnée Y
        RuleFor(b => b.Y)
            .GreaterThanOrEqualTo(0).WithMessage("La coordonnée Y doit être positive.")
            .LessThan(gridSize).WithMessage($"La coordonnée Y doit être inférieure à {gridSize}.");

        // Validation pour l'orientation
        RuleFor(b => b.Horizontal)
            .NotNull().WithMessage("L'orientation du bateau doit être définie.");

        // Validation conditionnelle en fonction de l'orientation
        When(b => b.Horizontal, () =>
        {
            RuleFor(b => b.X)
                .LessThanOrEqualTo(b => gridSize - b.Size) // Correction ici : utilisation de < au lieu de <=
                .WithMessage($"Le bateau dépasse les limites horizontales de la grille.");
        })
        .Otherwise(() =>
        {
            RuleFor(b => b.Y)
                .LessThanOrEqualTo(b => gridSize - b.Size) // Correction ici : utilisation de < au lieu de <=
                .WithMessage($"Le bateau dépasse les limites verticales de la grille.");
        });
    }
}