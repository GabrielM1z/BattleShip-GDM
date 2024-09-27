public class GameState
{
    // Grille du joueur (lettres pour les bateaux, ' ' pour vide)
    public char[,] PlayerGrid { get; set; }

    // Grille de l'adversaire (true = touché, false = raté, null = jamais tiré)
    public bool?[,] OpponentGrid { get; set; }

    // Initialisation des grilles
    public GameState()
    {
        PlayerGrid = new char[10, 10]; // Grille du joueur (10x10)
        OpponentGrid = new bool?[10, 10]; // Grille de l'adversaire (10x10)
    }
}
