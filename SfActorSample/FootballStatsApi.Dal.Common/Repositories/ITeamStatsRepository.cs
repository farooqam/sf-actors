using System.Threading.Tasks;
using FootballStatsApi.Dal.Common.Dto;

namespace FootballStatsApi.Dal.Common.Repositories
{
    public interface ITeamStatsRepository
    {
        Task<TeamStatsDto> GetTeamStatsAsync(string id);

        Task UpsertTeamStatsAsync(TeamStatsDto dto);
    }
}