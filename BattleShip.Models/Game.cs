
namespace BattleShip.Models
{
	public class Game
	{
		public Guid Id { get; set; } 
		public char[][] GridJ1 { get; set; }
		public char[][] GridJ2 { get; set; }
		public bool?[][] MaskedGridJ1 { get; set; }
		public bool?[][] MaskedGridJ2 { get; set; }
		public bool IsGameFinished { get; set ;}
		public int GameMode { get; set;}
		public int IaLvl { get; set;}
		public int PVE { get; set;}
		public Fleet fleetJ1 {get; set;}
		public Fleet fleetJ2 {get; set;}

		// Méthode pour afficher les informations du jeu
		public void PrintGame()
		{
			Console.WriteLine($"Game ID: {Id}");
			//PrintGrid(GridJ1, "Grille J1");
			//PrintGrid(GridJ2, "Grille J2");
			//PrintMaskedGrid(MaskedGridJ1, "Masked Grille J1");
			//PrintMaskedGrid(MaskedGridJ2, "Masked Grille J2");
			PrintGridsSideBySide(GridJ1, MaskedGridJ1, "Grille J1", "Masqué J1");
			PrintGridsSideBySide(GridJ2, MaskedGridJ2, "Grille J2", "Masqué J2");
		}

		// Méthode pour afficher une grille
		public void PrintGrid(char[][] gridArray, string gridName)
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
		// Méthode pour afficher la grille masquée
		public void PrintMaskedGrid(bool?[][] maskedGrid, string gridName)
		{
			Console.WriteLine($"Masque : {gridName}");
			Console.WriteLine(new string('-', (maskedGrid[0].Length * 2) + 1));

			for (int i = 0; i < maskedGrid.Length; i++)
			{
				Console.Write("|");
				for (int j = 0; j < maskedGrid[i].Length; j++)
				{
					//Console.WriteLine($"{i},{j},{maskedGrid[i][j]}");
					/*
					if (maskedGrid[i][j] == true)
					{
						Console.Write("X|"); // Bateau touché
					}
					else if (maskedGrid[i][j] == false)
					{
						Console.Write("O|"); // Tir raté
					}
					else
					{
						Console.Write(" |"); // Non révélé
					}
					*/
				}
				Console.WriteLine();
				Console.WriteLine(new string('-', (maskedGrid[0].Length * 2) + 1));
			}
		}

		// Méthode pour afficher la grille et la grille masquée côte à côte
		public void PrintGridsSideBySide(char[][] grid, bool?[][] maskedGrid, string gridName, string maskedGridName)
		{
			Console.WriteLine($"{gridName}                 {maskedGridName}");
			Console.WriteLine(new string('-', (grid[0].Length * 2) + 1) + "    " + new string('-', (maskedGrid[0].Length * 2) + 1));

			for (int i = 0; i < grid.Length; i++)
			{
				// Affichage de la grille
				Console.Write("|");
				for (int j = 0; j < grid[i].Length; j++)
				{
					if (grid[i][j] == '\0')
					{
						Console.Write(" |");
					}
					else
					{
						Console.Write($"{grid[i][j]}|");
					}
				}

				// Espacement entre les deux grilles
				Console.Write("    ");

				// Affichage de la grille masquée
				Console.Write("|");
				for (int j = 0; j < maskedGrid[i].Length; j++)
				{
					if (maskedGrid[i][j] == true)
					{
						Console.Write("X|"); // Bateau touché
					}
					else if (maskedGrid[i][j] == false)
					{
						Console.Write("O|"); // Tir raté
					}
					else
					{
						Console.Write(" |"); // Non révélé
					}
				}

				Console.WriteLine();
				Console.WriteLine(new string('-', (grid[0].Length * 2) + 1) + "    " + new string('-', (maskedGrid[0].Length * 2) + 1));
			}
		}

		public void SetGame(GameStateHisto game)
		{
			Id = game.Id;
			GridJ1 = game.GridJ1;
			GridJ2 = game.GridJ2;
			MaskedGridJ1 = game.MaskedGridJ1;
			MaskedGridJ2 = game.MaskedGridJ2;
			IsGameFinished = game.IsGameFinished;
			GameMode = game.GameMode;
			IaLvl = game.IaLvl;
			PVE = game.PVE;
			fleetJ1 = game.FleetJ1;
			fleetJ2 = game.FleetJ2;
		}
	}
}
