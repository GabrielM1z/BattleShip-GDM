namespace BattleShip.Models
{
    public class Grid
    {
        public int[,] Matrix { get; set; }

        public Grid(int size)
        {
            Matrix = new int[size, size];
        }
    }
}