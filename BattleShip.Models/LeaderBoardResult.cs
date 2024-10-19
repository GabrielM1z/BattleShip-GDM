public class LeaderBoardResult
{
    public List<KeyValuePair<string, int>> UserCoupCount { get; set; }
    public List<KeyValuePair<string, int>> UserVictoryCount { get; set; }

    public LeaderBoardResult()
    {
        UserCoupCount = new List<KeyValuePair<string, int>>();
        UserVictoryCount = new List<KeyValuePair<string, int>>();
    }
}
