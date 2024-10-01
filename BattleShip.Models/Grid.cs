namespace BattleShip.Models
{
    public class Grid
    {
        public char[][] GridArray { get; set; }
        public int Size { get; set; }

        public Grid(int size)
        {
            Size = size;
            GridArray = new char[Size][];
            for (int i = 0; i < Size; i++)
            {
                GridArray[i] = new char[Size];
            }
        }
    }
}
