using BattleShip.Models;

namespace BattleShip.API.Service
{
    public class GridService
    {
        private Random _random = new Random(); // Pour la génération aléatoire
        
        public Grid CreateGrid(int GridSize)
        {
            var grid = new Grid(GridSize); // Crée une grille de 10x10
            InitializeGrid(grid);
            //PlaceBoat(grid, boats);
            return grid;
        }

        public void InitializeGrid(Grid grid)
        {
            for (int i = 0; i < grid.Size; i++)
            {
                for (int j = 0; j < grid.Size; j++)
                {
                    grid.GridArray[i][j] = '\0';
                }
            }
        }


        public void PlaceBoat(char[][] grid, List<Boat> boats)
        {
            foreach (var boat in boats)
            {
                bool placed = false;

                // Vérifie si les coordonnées sont spécifiées
                if (boat.X != -1 && boat.Y != -1) // Si les coordonnées sont spécifiées
                {
                    int row = boat.Y;
                    int col = boat.X;
                    // Vérifie si le placement est valide
                    if (CanPlaceBoat(grid, boat, row, col, boat.Horizontal))
                    {
                        for (int i = 0; i < boat.Size; i++)
                        {
                            if (boat.Horizontal)
                                grid[row][col + i] = boat.Symbol; // Placer horizontalement
                            else
                                grid[row + i][col] = boat.Symbol; // Placer verticalement
                        }
                        placed = true;
                    }
                    else
                    {
                        throw new Exception($"Cannot place boat {boat.Symbol} at specified coordinates.");
                    }
                }

                // Si le bateau n'est pas encore placé, effectue un placement aléatoire
                if (!placed)
                {
                    while (!placed)
                    {
                        bool horizontal = _random.Next(2) == 0; // Choisir aléatoirement horizontal ou vertical
                        int row = _random.Next(grid.Length);
                        int col = _random.Next(grid[0].Length);
                        boat.Y = row;
                        boat.X = col;
                        if (CanPlaceBoat(grid, boat, row, col, horizontal))
                        {
                            for (int i = 0; i < boat.Size; i++)
                            {
                                if (horizontal)
                                    grid[row][col + i] = boat.Symbol; // Placer horizontalement
                                else
                                    grid[row + i][col] = boat.Symbol; // Placer verticalement
                            }
                            placed = true;
                        }
                    }
                }
            }
        }



        


        // Méthode pour vérifier si le bateau peut être placé
        private bool CanPlaceBoat(char[][] grid, Boat boat, int row, int col, bool horizontal)
        {
            if (horizontal)
            {
                if (col + boat.Size > grid.Length) return false;

                for (int i = 0; i < boat.Size; i++)
                {
                    if (grid[row][col + i] != '\0') return false; // Vérifie s'il y a un bateau déjà placé
                }
            }
            else
            {
                if (row + boat.Size > grid[0].Length) return false;

                for (int i = 0; i < boat.Size; i++)
                {
                    if (grid[row + i][col] != '\0') return false; // Vérifie s'il y a un bateau déjà placé
                }
            }

            return true;
        }

        public bool?[][] CreateMaskedGrid(char[][] gridArray)
        {
            int gridSize = gridArray.Length; // On déduit la taille de la grille
            var maskedGrid = new bool?[gridSize][];
            
            for (int i = 0; i < gridSize; i++)
            {
                maskedGrid[i] = new bool?[gridSize];
                
                for (int j = 0; j < gridSize; j++)
                {
                    if (gridArray[i][j] == 'X')
                    {
                        maskedGrid[i][j] = true; // Bateau touché
                    }
                    else if (gridArray[i][j] == 'O')
                    {
                        maskedGrid[i][j] = false; // Tir raté
                    }
                    else
                    {
                        maskedGrid[i][j] = null; // Non révélé
                    }
                }
            }

            return maskedGrid;
        }





        public ShootResult PlayerShoot(char[][] targetGrid, bool?[][] maskedGrid, int x, int y)
        {
            // Vérification des limites de la grille
            if (x < 0 || x >= targetGrid.Length || y < 0 || y >= targetGrid[0].Length)
            {
                throw new ArgumentOutOfRangeException("Coordinates are out of bounds.");
            }

            // Vérification si la case a déjà été visée
            if (maskedGrid[y][x].HasValue)
            {
                return new ShootResult { CanShoot = false , IsHit = false};
            }

            // Vérification s'il y a un bateau sur la grille normale
            var hit = targetGrid[y][x] != '\0';


            // Mise à jour de la grille masquée (true si touché, false si manqué)
            maskedGrid[y][x] = hit ? true : false;

            // Affichage de l'état du coup
            Console.WriteLine($"Shoot result at ({x}, {y}): {(hit ? "Hit" : "Miss")}");

            int lettre = y + 1;
            char chiffre = (char)('A' + x);

            // Retourneç le résultat du tir
            return new ShootResult { CanShoot = true, IsHit = hit, Message = hit ? $"Touché en {lettre}{chiffre}" : $"Raté en {lettre}{chiffre}"};
        }


        public bool IsGameFinished(char[][] grid, bool?[][] maskedGrid)
        {
            // Parcourt chaque ligne de la grille normale
            for (int i = 0; i < grid.Length; i++)
            {
                // Parcourt chaque cellule de la ligne
                for (int j = 0; j < grid[i].Length; j++)
                {
                    // Vérifie si le caractère est entre 'A' (65) et 'F' (70)
                    if (grid[i][j] >= 'A' && grid[i][j] <= 'F')
                    {
                        // Vérifie que la cellule correspondante dans la grille masquée n'est pas true
                        if (maskedGrid[i][j] != true)
                        {
                            return false; // Le jeu continue car un bateau est encore présent et non touché
                        }
                    }
                }
            }
            return true; // Tous les bateaux ont été touchés
        }



    }
}
