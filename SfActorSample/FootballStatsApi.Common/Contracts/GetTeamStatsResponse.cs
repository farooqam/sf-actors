namespace FootballStatsApi.Common.Contracts
{
    public class GetTeamStatsResponse
    {
        public short Year { get; set; }

        public byte Week { get; set; }

        public string TeamId { get; set; }

        public string TeamName { get; set; }

        public int PointsFor { get; set; }

        public int PointsAgainst { get; set; }

        public byte Wins { get; set; }

        public byte Losses { get; set; }

        public byte GamesPlayed { get; set; }
    }
}