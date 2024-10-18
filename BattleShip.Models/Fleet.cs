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
			AddBoat(new Boat(1, 2, 'A', "Torpilleur")); // Bateau de taille 2
			AddBoat(new Boat(2, 3, 'B', "Sous-marin")); // Bateau de taille 3
			AddBoat(new Boat(3, 3, 'C', "Fregatte")); // Bateau de taille 3
			AddBoat(new Boat(4, 4, 'D', "Croisseur")); // Bateau de taille 4
			AddBoat(new Boat(5, 5, 'E', "Porte avion")); // Bateau de taille 5
		}

		public void AddBoat(Boat boat)
		{
			//Console.WriteLine($"\nBoat = {boat.Name}\n");
			Boats.Add(boat);
		}

		// Méthode pour obtenir la liste des bateaux
		public List<Boat> GetBoats()
		{
			return Boats;
		}
		
		// Méthode pour obtenir la liste des bateaux sans IsAlive (/place)
		public List<object> GetBoatsWithoutIsAlive()
		{
			return Boats.Select(boat => new
			{
				boat.Id,
				boat.Name,
				boat.Size,
				boat.Symbol,
				boat.X,
				boat.Y,
				boat.Horizontal
			}).ToList<object>();
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
					return $"Coulé! Le {boat.Name} est hors sous l'océan.";
				}
				else
				{
					return "";
				}
			}
			return "";
		}
		public void SetBoatPosition(int boatId, int x, int y, bool horizontal)
		{
			Console.WriteLine($"SetBoatPosition");
			var boat = Boats.FirstOrDefault(b => b.Id == boatId);

			if (boat != null)
			{
				boat.X = x;
				boat.Y = y;
				boat.Horizontal = horizontal;

			}
			Console.WriteLine($"----PlaceBoat {boat.Id}= ({boat.X},{boat.Y})");
		}
	}

}
