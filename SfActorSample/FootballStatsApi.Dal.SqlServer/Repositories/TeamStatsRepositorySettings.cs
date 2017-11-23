using System;

namespace FootballStatsApi.Dal.SqlServer.Repositories
{
    public class TeamStatsRepositorySettings
    {
        public string ConnectionString { get; set; }

        public TimeSpan QueryTimeout { get; set; }
    }
}