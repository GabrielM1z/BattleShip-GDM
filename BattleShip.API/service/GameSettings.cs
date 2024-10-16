public interface IGameSettingsService
{
    GameSettings ParseGameSettings(string settings);
}

public class GameSettingsService : IGameSettingsService
{
    public GameSettings ParseGameSettings(string settings)
    {
        // S'assurer que la chaîne a exactement 2 caractères
        if (settings.Length != 2)
        {
            throw new ArgumentException("La chaîne doit contenir exactement 2 caractères.");
        }

        // Extraire les valeurs du string
        bool pve = settings[0] == '1'; // Premier caractère : PVE = true, sinon PVP
        int aiCode = int.Parse(settings[1].ToString()); // Deuxième caractère : niveau IA et taille de la grille

        int gridSize = 8; // Valeur par défaut
        int aiLevel = 1; // Valeur par défaut (niveau de l'IA)

        // Logique de détermination de la grille et du niveau de l'IA en fonction du code
        switch (aiCode)
        {
            case 0: 
                gridSize = 8; // Grille de taille 8
                aiLevel = 1; // Niveau IA 1
                break;
            case 1:
                gridSize = 8; // Grille de taille 8
                aiLevel = 2; // Niveau IA 2
                break;
            case 2:
                gridSize = 10; // Grille de taille 10
                aiLevel = 3; // Niveau IA 3
                break;
            case 3:
                gridSize = 10; // Grille de taille 10
                aiLevel = 4; // Niveau IA 4
                break;
            case 4:
                gridSize = 12; // Grille de taille 12
                aiLevel = 4; // Niveau IA 4
                break;
            default:
                throw new ArgumentException("Code de niveau IA non valide.");
        }

        return new GameSettings(gridSize, aiLevel, pve);
    }
}