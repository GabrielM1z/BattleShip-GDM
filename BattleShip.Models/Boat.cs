using System.Diagnostics;

public class Boat
{
    public int Size { get; }
    public char Symbol { get; }
    public (int X, int Y) Coordinates { get; set; } // Propriété pour les coordonnées
    public Boolean Horizontal { get; set; }

    public Boat(int size, char symbol)
    {
        Size = size;
        Symbol = symbol;
        Coordinates = (-1, -1); // Valeurs par défaut indiquant que les coordonnées sont inconnues
        Horizontal = true;
    }
}
