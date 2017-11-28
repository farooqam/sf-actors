using System;
using System.Threading.Tasks;
using FootballStatsActor.Interfaces;
using FootballStatsApi.Dal.Common.Dto;
using FootballStatsApi.Dal.Common.Repositories;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace FootballStatsApi.Dal.SfActor
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
            var actorId = new ActorId($"{id}/{year}/{week}".ToUpperInvariant());
            var actor = ActorProxy.Create<IFootballStatsActor>(actorId, _settings.ActorServiceUri);
            var dto = await actor.Get();
            return dto;
        }

        public async Task UpsertTeamStatsAsync(TeamStatsDto dto)
        {
            var actorId = new ActorId($"{dto.TeamId}/{dto.Year}/{dto.Week}".ToUpperInvariant());
            var actor = ActorProxy.Create<IFootballStatsActor>(actorId, _settings.ActorServiceUri);
            await actor.Update(dto);
        }
    }
}