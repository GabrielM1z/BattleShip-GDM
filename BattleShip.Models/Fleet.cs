using BattleShip.Models;

public class Fleet
{
    public List<Boat> Boats { get; set; }

    public Fleet(bool init)
    {
        Boats = new List<Boat>();
        if(init)
            InitializeBoats(); // Appelle la méthode pour initialiser les bateaux
    }

    // Méthode pour ajouter des bateaux à la flotte
    public void InitializeBoats()
    {
        AddBoat(new Boat(1, 2, 'A')); // Bateau de taille 2
        AddBoat(new Boat(2, 3, 'B')); // Bateau de taille 3
        AddBoat(new Boat(3, 3, 'C')); // Bateau de taille 3
        AddBoat(new Boat(4, 4, 'D')); // Bateau de taille 4
        AddBoat(new Boat(5, 5, 'E')); // Bateau de taille 5
    }

    public void AddBoat(Boat boat)
    {
        Boats.Add(boat);
    }

    // Méthode pour obtenir la liste des bateaux
    public List<Boat> GetBoats()
    {
        return Boats; // Retourne la liste des bateaux
    }
    public void UpdateBoats(char[][] Grid, bool?[][] masked)
    {
        if (masked == null) return; // Si la grille masked est null, on ne fait rien
        
        foreach (var boat in Boats)
        {
            bool isSunk = false; // Supposons que le bateau est touché complètement

            for (int i = 0; i < Grid.Length; i++) // Parcourt les lignes de la grille
            {
                for (int j = 0; j < Grid[i].Length; j++) // Parcourt les colonnes de la grille
                {
                    // Si la case actuelle contient le caractère du bateau
                    if (Grid[i][j] == boat.Symbol)
                    {
                        // Si masked à cet emplacement est faux, le bateau n'est pas encore complètement touché
                        if (masked[i][j] != true)
                        {
                            isSunk = true;
                            break; // Pas besoin de continuer à vérifier, ce bateau n'est pas coulé
                        }
                    }
                }
            }
            // Met à jour l'état du bateau (par exemple, définir une propriété `IsSunk`)
            boat.IsAlive = isSunk; // On suppose que `Boat` a une propriété `IsSunk`
        }
    }

}
