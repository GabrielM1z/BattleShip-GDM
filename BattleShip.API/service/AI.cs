using BattleShip.Models;

namespace BattleShip.API.Service
{
    public class AI
    {

        
        public (int, int) manage_call_ia(int ia, bool?[][] grid, Fleet fleet)
        {
            if(ia == 1){
                return GenerateValidIACoordinates_IA1(grid);
            }else if(ia == 2){
                return GenerateValidIACoordinates_IA2(grid);
            }else if(ia == 3){
                return GenerateValidIACoordinates_IA3(grid);
            }else if(ia == 4){
                return GenerateValidIACoordinates_IA4(grid, fleet);
            }else if(ia == 5){
                return GenerateValidIACoordinates_IA5(grid, fleet);
            }else{
                return GenerateValidIACoordinates_IA1(grid);
            }
        }  
        (int, int) GenerateValidIACoordinates_IA1(bool?[][] grid){
            for (int i = 0; i < grid.Length; i++)
                    {
                        for (int j = 0; j < grid[i].Length; j++)
                        {
                            if (grid[i][j]== null)
                            {
                                return (j,i);
                            }
                        }
                    }
            return (0,0);

        } 
        (int, int) GenerateValidIACoordinates_IA2(bool?[][] grid){
            Random random = new Random();
            int xIa, yIa;
            do
            {
                xIa = random.Next(grid.Length);
                yIa = random.Next(grid[0].Length);
            }
            while (grid[yIa][xIa] != null); 

            return (xIa, yIa);
        }           
        (int, int) GenerateValidIACoordinates_IA3(bool?[][] grid){
            for (int i = 0; i < grid.Length; i++)
                    {
                        for (int j = 0; j < grid[i].Length; j++)
                        {
                            if (grid[i][j] == true && CanShootAround(grid, i, j))
                            {
                                if (i > 0 && grid[i - 1][j] == null) // Vérifie la case au-dessus
                                    return (j,i-1);
                                
                                if (i < grid.Length - 1 && grid[i + 1][j] == null) // Vérifie la case en-dessous
                                    return (j,i+1);

                                if (j > 0 && grid[i][j - 1] == null) // Vérifie la case à gauche
                                    return (j-1,i);

                                if (j < grid[i].Length - 1 && grid[i][j + 1] == null) // Vérifie la case à droite
                                    return (j+1,i);
                            }
                        }
                    }
            return GenerateValidIACoordinates_IA1(grid);

        }
        (int, int) GenerateValidIACoordinates_IA4(bool?[][] grid, Fleet fleet){
            bool areAllBoatsSunk = true;  //il y a un bateau touché mais non coulé
            areAllBoatsSunk = CheckSinkBoat(grid, fleet);
            if(!areAllBoatsSunk){
                GenerateValidIACoordinates_IA3(grid);
            }

            int x, y;
            int nb_max_attempts = 15;
            for (int attempts = 0; attempts < nb_max_attempts; attempts++){
                (int, int) a = GenerateValidIACoordinates_IA1(grid);
                (x, y) = a;
                if (IsNotShootAround(grid, x, y)) 
                    return a;
            }
            return GenerateValidIACoordinates_IA1(grid);
            
        }
        (int, int) GenerateValidIACoordinates_IA5(bool?[][] grid, Fleet fleet){
            bool areAllBoatsSunk = true;  //il y a un bateau touché mais non coulé
            areAllBoatsSunk = CheckSinkBoat(grid, fleet);
            if(!areAllBoatsSunk){
                GenerateValidIACoordinates_IA3(grid);
            }

            int x, y;
            int nb_max_attempts = 15;
            for (int attempts = 0; attempts < nb_max_attempts; attempts++){
                (int, int) a = GenerateValidIACoordinates_IA1(grid);
                (x, y) = a;
                if (IsNotShootAround(grid, x, y)) 
                    return a;
            }
            return GenerateValidIACoordinates_IA1(grid);
            
        }


        bool CheckSinkBoat(bool?[][] grid, Fleet fleet)
        {
            int totalSunkBoatSize = fleet.Boats
                .Where(boat => !boat.IsAlive)   // Filtrer les bateaux coulés
                .Sum(boat => boat.Size);        // Additionner la taille des bateaux coulés

            int trueCountInGrid = grid.Sum(row => row.Count(cell => cell == true));
            //Console.WriteLine($"CanShootAround totalSunkBoatSize={totalSunkBoatSize}, trueCountInGrid={trueCountInGrid}");
            bool areAllBoatsSunk = (totalSunkBoatSize == trueCountInGrid);
            return areAllBoatsSunk;
        }

        bool CanShootAround(bool?[][] grid, int j, int i)
        {
            int nb = 0;
            // Vérifier les limites pour éviter les accès hors des bords de la grille
            if (i > 0 && grid[i - 1][j] == null) // Vérifie la case au-dessus
                nb++;
            
            if (i < grid.Length - 1 && grid[i + 1][j] == null) // Vérifie la case en-dessous
                nb++;

            if (j > 0 && grid[i][j - 1] == null) // Vérifie la case à gauche
                nb++;

            if (j < grid[i].Length - 1 && grid[i][j + 1] == null) // Vérifie la case à droite
                nb++;

            //PrintSurroundingCells(grid,i,j);
            // Si toutes les cases adjacentes sont null, retourner true
            if (nb > 0){
                return true;
            }else{
                return false;
            }
        }

        bool IsNotShootAround(bool?[][] grid, int j, int i)
        {
            int nb = 0;
            int possibility = 0;
            // Vérifier les limites pour éviter les accès hors des bords de la grille
            if (i > 0){
                possibility ++;
                if(grid[i - 1][j] == null)
                    nb++;
            }
            if (i < grid.Length - 1){
                possibility ++;
                if(grid[i + 1][j] == null)
                    nb++;
            }
            if (j > 0){
                possibility ++;
                if(grid[i][j - 1] == null)
                    nb++;
            }
            if (j < grid[i].Length - 1){
                possibility ++;
                if(grid[i][j + 1] == null)
                    nb++;
            }
            
            
            
            bool isEqual = possibility == nb ? true : false;
            //PrintSurroundingCells(grid,i,j);
            // Si toutes les cases adjacentes sont null, retourner true
            if (nb > 0 && isEqual){
                return true;
            }else{
                return false;
            }
        }

        void PrintSurroundingCells(bool?[][] grid, int i, int j)
        {


            string topLeft = GetGridValue(grid, i - 1, j - 1);
            string top = GetGridValue(grid, i - 1, j);
            string topRight = GetGridValue(grid, i - 1, j + 1);

            string left = GetGridValue(grid, i, j - 1);
            string center = GetGridValue(grid, i, j);
            string right = GetGridValue(grid, i, j + 1);

            string bottomLeft = GetGridValue(grid, i + 1, j - 1);
            string bottom = GetGridValue(grid, i + 1, j);
            string bottomRight = GetGridValue(grid, i + 1, j + 1);

            // Affichage
            Console.WriteLine($"|{topLeft}|{top}|{topRight}|");
            Console.WriteLine($"|{left}|{center}|{right}|");
            Console.WriteLine($"|{bottomLeft}|{bottom}|{bottomRight}|\n");
        }

        string GetGridValue(bool?[][] grid, int i, int j)
        {
            if (i < 0 || i >= grid.Length || j < 0 || j >= grid[i].Length)
                return " "; // Hors de la grille

            return grid[i][j] == true ? "T" :
                grid[i][j] == false ? "F" : " "; // T pour true, F pour false, X pour null
        }

    }
}
