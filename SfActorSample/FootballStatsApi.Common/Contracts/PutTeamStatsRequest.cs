namespace FootballStatsApi.Common.Contracts
{
    public class PutTeamStatsRequest
    {
        public short Year { get; set; }

        public string TeamId { get; set; }

        public string TeamName { get; set; }

        public short PointsFor { get; set; }

        public short PointsAgainst { get; set; }

        public byte Wins { get; set; }

        public byte Losses { get; set; }

        public byte GamesPlayed { get; set; }
    }
}