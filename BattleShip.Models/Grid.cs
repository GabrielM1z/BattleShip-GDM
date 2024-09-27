using System.Dynamic;

namespace BattleShip.Models
{
    public class Grid
    {
        public char[,] Matrix { get; set; }
        public int Size { get; set; }

        public Grid(int size)
        {
            Size = size;
            Matrix = new char[Size, Size];
        }
    }
}