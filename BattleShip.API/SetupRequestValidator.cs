using BattleShip.Models;
using FluentValidation;

public class SetupRequestValidator : AbstractValidator<SetupRequest>
{
    public SetupRequestValidator()
    {
        string[] validDifficulties = { "00", "01", "02", "10", "11", "12", "13", "14" };

        RuleFor(x => x.LevelDifficulty)
            .NotEmpty().WithMessage("Le niveau de difficulté doit être fourni.")
            .Must(value => validDifficulties.Contains(value))
            .WithMessage("Le niveau de difficulté doit être l'un des formats acceptés : 00, 01, 02, 10, 11, 12, 13, 14.");

    }
}