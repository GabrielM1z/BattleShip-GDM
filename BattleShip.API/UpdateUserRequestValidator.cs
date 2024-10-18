using FluentValidation;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        // Validation du nom de l'utilisateur
        RuleFor(request => request.User)
            .NotEmpty().WithMessage("Le nom d'utilisateur est obligatoire.");

        // Validation du nombre de coups
        RuleFor(request => request.NbCoup)
            .GreaterThanOrEqualTo(17).WithMessage("Le nombre de coups doit être au moins 17.");
    }
}