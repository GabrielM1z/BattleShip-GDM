using BattleShip.Models;

namespace BattleShip.Models
{
	public class Fleet
	{
		public List<Boat> Boats { get; set; }

		public Fleet()
		{
			Boats = new List<Boat>();
		}

		public Fleet(bool init)
		{
			Boats = new List<Boat>();
			if(init)
				InitializeBoats(); // Appelle la méthode pour initialiser les bateaux
		}

		// Méthode pour ajouter des bateaux à la flotte
		public void InitializeBoats()
		{
			AddBoat(new Boat(1, 2, 'A')); // Bateau de taille 2
			AddBoat(new Boat(2, 3, 'B')); // Bateau de taille 3
			AddBoat(new Boat(3, 3, 'C')); // Bateau de taille 3
			AddBoat(new Boat(4, 4, 'D')); // Bateau de taille 4
			AddBoat(new Boat(5, 5, 'E')); // Bateau de taille 5
		}

		public void AddBoat(Boat boat)
		{
			Boats.Add(boat);
		}

		// Méthode pour obtenir la liste des bateaux
		public List<Boat> GetBoats()
		{
			return Boats;
		}
		public void UpdateBoats(char[][] Grid, bool?[][] masked)
		{
			if (masked == null) return;
			
			foreach (var boat in Boats)
			{
				bool isSunk = false;

				for (int i = 0; i < Grid.Length; i++)
				{
					for (int j = 0; j < Grid[i].Length; j++)
					{
						if (Grid[i][j] == boat.Symbol)
						{
							if (masked[i][j] != true)
							{
								isSunk = true;
								break;
							}
						}
					}
				}
				boat.IsAlive = isSunk;
			}
		}

		public string IsShootSink(char lettre_shoot)
		{
			var boat = Boats.FirstOrDefault(b => b.Symbol == lettre_shoot);
			if (boat != null)
			{
				if (!boat.IsAlive)
				{
					return $"Coulé! Le navire {boat.Symbol} est hors service.";
				}
				else
				{
					return "";
				}
			}
			return "";
		}


	}
}
