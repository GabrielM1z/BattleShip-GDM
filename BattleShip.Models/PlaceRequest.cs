namespace BattleShip.Models
{
    public class PlaceRequest
    {
        public int GridSize { get; set; }
        public string LevelDifficulty { get; set; }
        public List<Boat>? Boats { get; set; }
    }
}