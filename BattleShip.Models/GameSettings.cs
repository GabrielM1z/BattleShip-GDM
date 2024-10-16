public class GameSettings
{
    public int GridSize { get; set; }
    public int Level { get; set; }
    public bool PVE { get; set; }

    public GameSettings(int gridSize, int level, bool pve)
    {
        GridSize = gridSize;
        Level = level;
        PVE = pve;
    }
}