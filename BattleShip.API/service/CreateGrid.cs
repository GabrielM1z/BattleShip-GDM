using BattleShip.Models;

namespace BattleShip.API.Service
{
    public class GridService
    {
        public Grid CreateGrid()
        {
            var grid = new Grid(6); // Crée une grille de 10x10
            InitializeGrid(grid);
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
    }
}
