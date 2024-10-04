public class Fleet
{
    public List<Boat> Boats { get; }

    public Fleet()
    {
        Boats = new List<Boat>();
        InitializeBoats(); // Appelle la méthode pour initialiser les bateaux
    }

    // Méthode pour ajouter des bateaux à la flotte
    private void InitializeBoats()
    {
        AddBoat(new Boat(1, 2, 'A'));
        AddBoat(new Boat(2, 3, 'B'));
        AddBoat(new Boat(3, 3, 'C'));
        AddBoat(new Boat(4, 4, 'D'));
        AddBoat(new Boat(5, 5, 'E'));
        
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
}
