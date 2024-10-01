public class GameState
{
    // Grille du joueur (lettres pour les bateaux, ' ' pour vide)
    public char[][] GridJ1 { get; set; }

    // Grille de l'adversaire (true = touché, false = raté, null = jamais tiré)
    public bool?[][] MaskedGridJ2 { get; set; }

    // partie commencé ?
    public bool start = false;
    public bool end = false;

    // Initialisation des grilles
    public GameState()
    {
        // Initialiser les grilles avec des tableaux de 10x10
        GridJ1 = new char[10][];
        MaskedGridJ2 = new bool?[10][];
        
        for (int i = 0; i < 10; i++)
        {
            GridJ1[i] = new char[10];
            MaskedGridJ2[i] = new bool?[10];
            
            // Remplir la grille du joueur avec des espaces vides ' '
            for (int j = 0; j < 10; j++)
            {
                GridJ1[i][j] = '\0'; // ou ' ' si tu veux un espace vide
                MaskedGridJ2[i][j] = null; // Pas encore de tir sur cette grille
            }
        }
    }
}
