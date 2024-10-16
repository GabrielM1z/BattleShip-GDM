using BattleShip.Models;
using FluentValidation;

public class PlaceRequestValidator : AbstractValidator<PlaceRequest>
{
    public PlaceRequestValidator(IValidator<Boat> boatValidator)
    {
        // Validation des bateaux avec BoatValidator
        RuleForEach(x => x.Boats).SetValidator(boatValidator);

        // Validation pour la liste des bateaux
        RuleFor(x => x.Boats)
            .Must(boats => boats != null && boats.Count == 5).WithMessage("La flotte doit contenir 5 bateaux.")
            .Must(boats => boats != null && HaveCorrectBoatSizes(boats)).WithMessage("La flotte doit contenir un bateau de taille 2, deux de taille 3, un de taille 4, et un de taille 5.")
            .Must(boats => boats != null && !DoBoatsOverlap(boats)).WithMessage("Les bateaux ne doivent pas se chevaucher.");
    }

    // Méthode pour vérifier que la flotte contient les bateaux de tailles correctes
    private bool HaveCorrectBoatSizes(List<Boat> boats)
    {
        var sizeCounts = new Dictionary<int, int>
        {
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
            { 5, 0 }
        };

        foreach (var boat in boats)
        {
            if (sizeCounts.ContainsKey(boat.Size))
            {
                sizeCounts[boat.Size]++;
            }
        }

        return sizeCounts[2] == 1 && sizeCounts[3] == 2 && sizeCounts[4] == 1 && sizeCounts[5] == 1;
    }

    // Méthode pour vérifier si les bateaux se chevauchent
    private bool DoBoatsOverlap(List<Boat> boats)
    {
        var occupiedCoordinates = new HashSet<(int, int)>();

        foreach (var boat in boats)
        {
            // Récupération des coordonnées occupées par le bateau
            var boatCoordinates = GetBoatCoordinates(boat);

            // Vérification des chevauchements
            foreach (var coord in boatCoordinates)
            {
                if (occupiedCoordinates.Contains(coord))
                {
                    return true; // Chevauchement détecté
                }
                occupiedCoordinates.Add(coord);
            }
        }
        return false; // Pas de chevauchement
    }

    // Méthode pour obtenir les coordonnées occupées par un bateau
    private IEnumerable<(int, int)> GetBoatCoordinates(Boat boat)
    {
        var coordinates = new List<(int, int)>();
        if (boat.Horizontal)
        {
            for (int i = 0; i < boat.Size; i++)
            {
                coordinates.Add((boat.X + i, boat.Y));
            }
        }
        else
        {
            for (int i = 0; i < boat.Size; i++)
            {
                coordinates.Add((boat.X, boat.Y + i));
            }
        }
        return coordinates;
    }   
}