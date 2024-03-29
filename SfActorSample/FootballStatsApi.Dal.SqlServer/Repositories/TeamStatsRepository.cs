﻿using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FootballStatsApi.Dal.Common.Dto;
using FootballStatsApi.Dal.Common.Repositories;

namespace FootballStatsApi.Dal.SqlServer.Repositories
{
    public class TeamStatsRepository : ITeamStatsRepository
    {
        private readonly TeamStatsRepositorySettings _settings;

        public TeamStatsRepository(TeamStatsRepositorySettings settings)
        {
            _settings = settings;
        }

        public async Task<TeamStatsDto> GetTeamStatsAsync(string id, short year, byte week)
        {
            var query = @"SELECT [TeamId],
                            [Year],
                            [Week],
                            [TeamName],
                            [PointsFor],
                            [PointsAgainst],
                            [Wins],
                            [Losses],
                            [GamesPlayed]
                        FROM [dbo].[TeamStats] 
                        WHERE [TeamId] = @TeamId
                        AND [Year] = @Year
                        AND [Week] = @Week";

            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<TeamStatsDto>(
                    query,
                    new {TeamId = id, Year = year, Week = week},
                    commandTimeout: (int) _settings.QueryTimeout.TotalSeconds);

                return result.SingleOrDefault();
            }
        }

        public async Task UpsertTeamStatsAsync(TeamStatsDto dto)
        {
            var query = @"MERGE [dbo].[TeamStats] as target
                            using (SELECT @TeamId, @Year, @Week, @TeamName, @PointsFor, @PointsAgainst, @Wins, @Losses, @GamesPlayed) AS source
                                    ([TeamId], [Year], [Week], [TeamName], [PointsFor], [PointsAgainst], [Wins], [Losses], [GamesPlayed])
                            ON (target.[TeamId] = source.[TeamId])
                            WHEN MATCHED THEN
                                UPDATE SET
                                    [TeamId] = source.[TeamId],
                                    [Year] = source.[Year],
                                    [Week] = source.[Week],
                                    [TeamName] = source.[TeamName],
                                    [PointsFor] = source.[PointsFor],
                                    [PointsAgainst] = source.[PointsAgainst],
                                    [Wins] = source.[Wins],
                                    [Losses] = source.[Losses],
                                    [GamesPlayed] = source.[GamesPlayed],
                                    [LastUpdated] = getutcdate()
                            WHEN NOT MATCHED THEN
                                INSERT ([TeamId], [Year], [Week], [TeamName], [PointsFor], [PointsAgainst], [Wins], [Losses], [GamesPlayed], [Created])
                                VALUES (source.[TeamId], source.[Year], source.[Week], source.[TeamName], source.[PointsFor], source.[PointsAgainst], source.[Wins], source.[Losses], source.[GamesPlayed], getutcdate());";

            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                await connection.OpenAsync();

                await connection.ExecuteScalarAsync(
                    query,
                    new
                    {
                        dto.TeamId,
                        dto.Year,
                        dto.Week,
                        dto.TeamName,
                        dto.PointsFor,
                        dto.PointsAgainst,
                        dto.Wins,
                        dto.Losses,
                        dto.GamesPlayed
                    },
                    commandTimeout: (int) _settings.QueryTimeout.TotalSeconds);
            }
        }
    }
}