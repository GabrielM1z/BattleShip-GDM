using BattleShip.Models;

public class GameState
{
    public event Action OnChange;

    // Grille du joueur (lettres pour les bateaux, ' ' pour vide)
    public char[][] GridJ1 { get; set; }

    // Grille du joueur (en mode masqué)
    public bool?[][] MaskedGridJ1 { get; set; }

    // Grille de l'adversaire (true = touché, false = raté, null = jamais tiré)
    public bool?[][] MaskedGridJ2 { get; set; }

    // partie commencé ?
    public bool start = false;
    public bool end = false;

    // Etat des bateaux
    public Fleet fleetJ1 {get; set;}
	public Fleet fleetJ2 {get; set;}

    // Initialisation des grilles
    public GameState(int size)
    {
        // Initialiser les grilles avec des tableaux de 10x10
        GridJ1 = new char[size][];
        MaskedGridJ1 = new bool?[size][];
        MaskedGridJ2 = new bool?[size][];
        
        for (int i = 0; i < size; i++)
        {
            GridJ1[i] = new char[size];
            MaskedGridJ1[i] = new bool?[size];
            MaskedGridJ2[i] = new bool?[size];
            
            // Remplir la grille du joueur avec des espaces vides ' '
            for (int j = 0; j < size; j++)
            {
                GridJ1[i][j] = '\0'; // ou ' ' si tu veux un espace vide
                MaskedGridJ1[i][j] = null; // Pas encore de tir sur cette grille
                MaskedGridJ2[i][j] = null; // Pas encore de tir sur cette grille
            }
        }
    }

    public void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}
