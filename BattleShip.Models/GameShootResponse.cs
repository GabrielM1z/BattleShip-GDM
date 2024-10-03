namespace BattleShip.Models
{
    public class GameShootResponse
	{
		public BattleShip.Models.Game game { get; set; }
		public BattleShip.Models.ShootResult shootResultJ1 { get; set; }
		public BattleShip.Models.ShootResult shootResultJ2 { get; set; }
	}
}