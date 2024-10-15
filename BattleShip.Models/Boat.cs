using System.Diagnostics;

namespace BattleShip.Models
{
	public class Boat
	{
		public int Id { get; set;}
		public int Size { get; set;}
		public char Symbol { get; set;}
		public int X { get; set;}
		public int Y { get; set;}
		public string Name {get; set;}
		public Boolean Horizontal { get; set; }
		public Boolean IsAlive { get; set; }
		
		public Boat(int id, int size, char symbol, string name)
		{
			Id = id;
			Name = name;
			Size = size;
			Symbol = symbol;
			X = -1;
			Y = -1;
			Horizontal = true;
			IsAlive = true;
		}

	}
}
