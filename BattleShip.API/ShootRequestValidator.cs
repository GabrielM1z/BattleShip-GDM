using BattleShip.Models;
using FluentValidation;

public class ShootRequestValidator : AbstractValidator<ShootRequest>
{
    public ShootRequestValidator(Game game)
    {
        var gridSize = game.GridJ1.Length;

        RuleFor(x => x.X)
            .GreaterThanOrEqualTo(0).WithMessage("La coordonnée X doit être positive.")
            .LessThan(gridSize).WithMessage($"La coordonnée X doit être inférieure à {gridSize}.");

        RuleFor(x => x.Y)
            .GreaterThanOrEqualTo(0).WithMessage("La coordonnée Y doit être positive.")
            .LessThan(gridSize).WithMessage($"La coordonnée Y doit être inférieure à {gridSize}.");

        RuleFor(x => x.J)
            .InclusiveBetween(1, 2).WithMessage("Le joueur doit être 1 ou 2.");
    }
}