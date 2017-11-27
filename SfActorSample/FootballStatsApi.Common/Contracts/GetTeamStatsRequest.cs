namespace FootballStatsApi.Common.Contracts
{
    public class GetTeamStatsRequest
    {
        public string Id { get; set; }
        public short Year { get; set; }
        public byte Week { get; set; }
    }
}