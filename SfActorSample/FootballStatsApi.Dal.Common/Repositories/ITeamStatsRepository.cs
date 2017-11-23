using System.Collections.Generic;
using System.Threading.Tasks;
using FootballStatsApi.Dal.Common.Dto;

namespace FootballStatsApi.Dal.Common.Repositories
{
    public interface ITeamStatsRepository
    {
        Task<IEnumerable<TeamStatsDto>> GetTeamStatsAsync(string id);

        Task UpsertTeamStatsAsync(TeamStatsDto dto);
    }
}