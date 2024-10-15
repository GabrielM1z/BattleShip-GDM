namespace BattleShip.Models
{
    public class PlaceRequest
    {
        public string LevelDifficulty { get; set; }
        public List<Boat>? Boats { get; set; }
    }
}