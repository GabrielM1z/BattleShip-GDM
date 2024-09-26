namespace BattleShip.Models
{
    public class Grid
    {
        public char[,] Matrix { get; set; }

        public Grid(int size)
        {
            Matrix = new char[size, size];
        }
    }
}