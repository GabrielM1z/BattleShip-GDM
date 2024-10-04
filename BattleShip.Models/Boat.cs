using System.Diagnostics;

public class Boat
{
    public int Id { get; set;}
    public int Size { get; }
    public char Symbol { get; }
    public (int X, int Y) Coordinates { get; set; } // Propriété pour les coordonnées
    public Boolean Horizontal { get; set; }
    public Boolean IsAlive { get; set; }

    public Boat(int id, int size, char symbol)
    {
        Id = id;
        Size = size;
        Symbol = symbol;
        Coordinates = (-1, -1); // Valeurs par défaut indiquant que les coordonnées sont inconnues
        Horizontal = true;
        IsAlive = true;
    }
}
