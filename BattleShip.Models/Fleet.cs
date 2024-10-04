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
}
