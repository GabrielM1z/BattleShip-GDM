using BattleShip.Models;

namespace BattleShip.API.Service
{
    public class GridService
    {
        private Random _random = new Random(); // Pour la génération aléatoire
        
        public Grid CreateGrid()
        {
            var grid = new Grid(3); // Crée une grille de 10x10
            InitializeGrid(grid);
            PlaceBoat(grid);
            return grid;
        }

        public void PrintGrid(char[][] gridArray, string gridName)
        {
            Console.WriteLine($"Grille : {gridName}");
            Console.WriteLine(new string('-', (gridArray[0].Length * 2) + 1)); // Ligne de séparation

            for (int i = 0; i < gridArray.Length; i++)
            {
                Console.Write("|"); // Début de la ligne
                for (int j = 0; j < gridArray[i].Length; j++)
                {
                    // Affiche chaque élément et aligne les éléments avec des espaces
                    
                    if (gridArray[i][j] == '\0'){
                        Console.Write($" |");
                    }else{
                        Console.Write($"{gridArray[i][j]}|");
                    }
                }
                Console.WriteLine(); // Fin de la ligne
                Console.WriteLine(new string('-', (gridArray[0].Length * 2) + 1)); // Ligne de séparation
            }
        }




        private void InitializeGrid(Grid grid)
        {
            for (int i = 0; i < grid.Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < grid.Matrix.GetLength(1); j++)
                {
                    grid.Matrix[i, j] = '\0'; // Remplit la grille avec '\0'
                }
            }
        }

        public char[][] GetGridArray(Grid grid)
        {
            // Conversion de la matrice 2D en tableau de tableaux
            char[][] gridArray = new char[grid.Matrix.GetLength(0)][];
            for (int i = 0; i < grid.Matrix.GetLength(0); i++)
            {
                gridArray[i] = new char[grid.Matrix.GetLength(1)];
                for (int j = 0; j < grid.Matrix.GetLength(1); j++)
                {
                    gridArray[i][j] = grid.Matrix[i, j];
                }
            }

            return gridArray;
        }

        public bool?[][] GetBoolGridArray(bool?[,] grid)
    {
        // Conversion de la matrice 2D en tableau de tableaux
        bool?[][] gridArray = new bool?[grid.GetLength(0)][];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            gridArray[i] = new bool?[grid.GetLength(1)];
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                gridArray[i][j] = grid[i, j];
            }
        }

        return gridArray;
    }


        

        public void PlaceBoat(Grid grid)
        {
            // Définir les bateaux à placer (taille et symbole)
            var boats = new List<Boat>
            {
                /*
                new Boat(4, 'A'), // Bateau de taille 4
                new Boat(3, 'B'), // Bateau de taille 3
                new Boat(3, 'C'), // Bateau de taille 3
                new Boat(2, 'D'), // Bateau de taille 2
                new Boat(2, 'E'), // Bateau de taille 2
                */
                new Boat(1, 'A'),  // Bateau de taille 1
                new Boat(1, 'F')  // Bateau de taille 1
            };

            foreach (var boat in boats)
            {
                bool placed = false;

                while (!placed)
                {
                    // Choisir aléatoirement la direction (0 = horizontal, 1 = vertical)
                    bool horizontal = _random.Next(2) == 0;

                    // Choisir une position de départ aléatoire
                    int row = _random.Next(grid.Size);
                    int col = _random.Next(grid.Size);

                    // Vérifier si le bateau peut être placé
                    if (CanPlaceBoat(grid, boat, row, col, horizontal))
                    {
                        // Placer le bateau
                        for (int i = 0; i < boat.Size; i++)
                        {
                            if (horizontal)
                                grid.Matrix[row, col + i] = boat.Symbol; // Placer horizontalement
                            else
                                grid.Matrix[row + i, col] = boat.Symbol; // Placer verticalement
                        }
                        placed = true; // Bateau placé avec succès
                    }
                }
            }
        }

        // Méthode pour vérifier si le bateau peut être placé
        private bool CanPlaceBoat(Grid grid, Boat boat, int row, int col, bool horizontal)
        {
            // Vérifier les limites de la grille
            if (horizontal)
            {
                if (col + boat.Size > grid.Size) return false; // Dépassement à droite
            }
            else
            {
                if (row + boat.Size > grid.Size) return false; // Dépassement en bas
            }

            // Vérifier les cases occupées
            for (int i = 0; i < boat.Size; i++)
            {
                if (horizontal)
                {
                    if (grid.Matrix[row, col + i] != '\0') return false; // Case déjà occupée
                }
                else
                {
                    if (grid.Matrix[row + i, col] != '\0') return false; // Case déjà occupée
                }
            }

            return true; // Bateau peut être placé
        }

        public class ShootResult
        {
            public bool IsHit { get; set; }
            public bool CanShoot { get; set; }
        }

        public ShootResult PlayerShoot(char[][] targetGrid, int x, int y)
        {
            if (x < 0 || x >= targetGrid.Length || y < 0 || y >= targetGrid[0].Length)
            {
                throw new ArgumentOutOfRangeException("Coordinates are out of bounds.");
            }

            if (targetGrid[y][x] == 'X' || targetGrid[y][x] == 'O')
            {
                return new ShootResult { CanShoot = false };
            }

            var hit = targetGrid[y][x] != '\0';
            if (hit)
            {
                targetGrid[y][x] = 'X';
            }
            else
            {
                targetGrid[y][x] = 'O';
            }

            Console.WriteLine("shoot", targetGrid[y][x]);
            return new ShootResult { CanShoot = true, IsHit = hit };
        }

        public bool IsGameFinished(char[][] grid)
        {
            foreach (var row in grid)
            {
                foreach (var cell in row)
                {
                    // Vérifie si le caractère est entre 'A' (65) et 'F' (70)
                    if (cell >= 65 && cell <= 70) // plus simple a check selon moi
                    {
                        return false; // ça joue ncore
                    }
                }
            }
            return true;
        }

        public bool?[,] MaskedGrid(Grid grid)
        {
            // Crée une nouvelle grille avec des bool? (true, false, null)
            bool?[,] maskedGrid = new bool?[grid.Size, grid.Size];

            for (int i = 0; i < grid.Size; i++)
            {
                for (int j = 0; j < grid.Size; j++)
                {
                    if (grid.Matrix[i, j] == 'X')
                    {
                        // Bateau touché
                        maskedGrid[i, j] = true;
                    }
                    else if (grid.Matrix[i, j] == 'O')
                    {
                        // Tir raté
                        maskedGrid[i, j] = false;
                    }
                    else
                    {
                        // Case non révélée (bateau ou vide)
                        maskedGrid[i, j] = null;
                    }
                }
            }

            return maskedGrid;
        }



    }
}
