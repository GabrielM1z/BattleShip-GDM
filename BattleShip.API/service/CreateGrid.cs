using BattleShip.Models;

namespace BattleShip.API.Service
{
    public class GridService
    {
        private Random _random = new Random(); // Pour la génération aléatoire
        
        public Grid CreateGrid()
        {
            var grid = new Grid(10); // Crée une grille de 10x10
            InitializeGrid(grid);
            PlaceBoat(grid);
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


        public void PlaceBoat(Grid grid)
        {
            //*
            var boats = new List<Boat>
            {
                new Boat(4, 'A'),
                new Boat(3, 'B'),
                new Boat(3, 'C'),
                new Boat(2, 'D'),
                new Boat(2, 'E')
            };
            /*/
            var boats = new List<Boat>
            {
                new Boat(1, 'A'),
                new Boat(3, 'B'),
                new Boat(3, 'C'),
                new Boat(1, 'F')
            };
            //*/

            foreach (var boat in boats)
            {
                bool placed = false;

                while (!placed)
                {
                    bool horizontal = _random.Next(2) == 0; // Choisir aléatoirement horizontal ou vertical
                    int row = _random.Next(grid.Size);
                    int col = _random.Next(grid.Size);

                    if (CanPlaceBoat(grid, boat, row, col, horizontal))
                    {
                        for (int i = 0; i < boat.Size; i++)
                        {
                            if (horizontal)
                                grid.GridArray[row][col + i] = boat.Symbol; // Placer horizontalement
                            else
                                grid.GridArray[row + i][col] = boat.Symbol; // Placer verticalement
                        }
                        placed = true;
                    }
                }
            }
        }

        


        // Méthode pour vérifier si le bateau peut être placé
        private bool CanPlaceBoat(Grid grid, Boat boat, int row, int col, bool horizontal)
        {
            if (horizontal)
            {
                if (col + boat.Size > grid.Size) return false;

                for (int i = 0; i < boat.Size; i++)
                {
                    if (grid.GridArray[row][col + i] != '\0') return false; // Vérifie s'il y a un bateau déjà placé
                }
            }
            else
            {
                if (row + boat.Size > grid.Size) return false;

                for (int i = 0; i < boat.Size; i++)
                {
                    if (grid.GridArray[row + i][col] != '\0') return false; // Vérifie s'il y a un bateau déjà placé
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
                return new ShootResult { CanShoot = false };
            }

            // Vérification s'il y a un bateau sur la grille normale
            var hit = targetGrid[y][x] != '\0';


            // Mise à jour de la grille masquée (true si touché, false si manqué)
            maskedGrid[y][x] = hit ? true : false;

            // Affichage de l'état du coup
            Console.WriteLine($"Shoot result at ({x}, {y}): {(hit ? "Hit" : "Miss")}");

            // Retourne le résultat du tir
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
                        return false; // ça joue encore
                    }
                }
            }
            return true;
        }


    }
}
