public class LeaderBoardResult
{
    public Dictionary<string, int> UserCoupCount { get; set; }
    public Dictionary<string, int> UserVictoryCount { get; set; }

    public LeaderBoardResult()
    {
        UserCoupCount = new Dictionary<string, int>();
        UserVictoryCount = new Dictionary<string, int>();
    }
}
