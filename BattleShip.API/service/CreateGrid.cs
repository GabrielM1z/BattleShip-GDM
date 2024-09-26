using BattleShip.Models;

namespace BattleShip.API.Service
{
    public class GridService
    {
        private Random _random = new Random(); // Pour la génération aléatoire
        
        public Grid CreateGrid()
        {
            var grid = new Grid(4); // Crée une grille de 10x10
            InitializeGrid(grid);
            //PlaceBoat(grid);
            return grid;
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

        

        public void PlaceBoat(Grid grid)
        {
            // Définir les bateaux à placer (taille et symbole)
            var boats = new List<Boat>
            {
                new Boat(4, 'A'), // Bateau de taille 4
                new Boat(3, 'B'), // Bateau de taille 3
                new Boat(3, 'C'), // Bateau de taille 3
                new Boat(2, 'D'), // Bateau de taille 2
                new Boat(2, 'E'), // Bateau de taille 2
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

    }
}
