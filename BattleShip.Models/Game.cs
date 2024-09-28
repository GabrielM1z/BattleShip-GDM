
namespace BattleShip.Models
{
    public class Game
    {
        public Guid Id { get; set; } 
        public char[][] GridJ1 { get; set; }
        public char[][] GridJ2 { get; set; }
        public bool?[][] MaskedGridJ1 { get; set; }
        public bool?[][] MaskedGridJ2 { get; set; }

        // Méthode pour afficher les informations du jeu
        public void PrintGame()
        {
            Console.WriteLine($"Game ID: {Id}");
            PrintGrid(GridJ1, "Grille J1");
            PrintGrid(GridJ2, "Grille J2");
        }

        // Méthode pour afficher une grille
        private void PrintGrid(char[][] gridArray, string gridName)
        {
            Console.WriteLine($"Grille : {gridName}");
            Console.WriteLine(new string('-', (gridArray[0].Length * 2) + 1)); 

            for (int i = 0; i < gridArray.Length; i++)
            {
                Console.Write("|"); 
                for (int j = 0; j < gridArray[i].Length; j++)
                {
                    if (gridArray[i][j] == '\0')
                    {
                        Console.Write(" |");
                    }
                    else
                    {
                        Console.Write($"{gridArray[i][j]}|");
                    }
                }
                Console.WriteLine(); 
                Console.WriteLine(new string('-', (gridArray[0].Length * 2) + 1)); 
            }
        }
    }
}
