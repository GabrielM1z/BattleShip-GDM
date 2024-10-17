namespace BattleShip.Models
{
public class GameStateHisto
{
    public Guid Id { get; set; } 
    public char[][] GridJ1 { get; set; }
    public char[][] GridJ2 { get; set; }
    public bool?[][] MaskedGridJ1 { get; set; }
    public bool?[][] MaskedGridJ2 { get; set; }
    public bool IsGameFinished { get; set; }
    public int GameMode { get; set; }
    public int IaLvl { get; set; }
    public int PVE { get; set; }
    public Fleet FleetJ1 { get; set; }
    public Fleet FleetJ2 { get; set; }

    public GameStateHisto(Game game)
    {
        Id = game.Id;
        GridJ1 = CloneGrid(game.GridJ1);
        GridJ2 = CloneGrid(game.GridJ2);
        MaskedGridJ1 = CloneMaskedGrid(game.MaskedGridJ1);
        MaskedGridJ2 = CloneMaskedGrid(game.MaskedGridJ2);
        IsGameFinished = game.IsGameFinished;
        GameMode = game.GameMode;
        IaLvl = game.IaLvl;
        PVE = game.PVE;
        FleetJ1 = CloneFleet(game.fleetJ1);
        FleetJ2 = CloneFleet(game.fleetJ2);
    }

    // Méthodes pour cloner les grilles et les flottes
    private char[][] CloneGrid(char[][] originalGrid) => originalGrid.Select(row => row.ToArray()).ToArray();
    private bool?[][] CloneMaskedGrid(bool?[][] originalMaskedGrid) => originalMaskedGrid.Select(row => row.ToArray()).ToArray();
    private Fleet CloneFleet(Fleet originalFleet)
    {
        Fleet boats = new Fleet(true);
        if(originalFleet.Boats.Count > 0){
            boats.Boats = originalFleet.Boats;
        }

        return boats;
    }
}

}