using System.Threading.Tasks;
using FootballStatsActor.Interfaces;
using FootballStatsApi.Dal.Common.Dto;
using FootballStatsApi.Dal.Common.Repositories;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace FootballStatsActor
{
    /// <remarks>
    ///     This class represents an actor.
    ///     Every ActorID maps to an instance of this class.
    ///     The StatePersistence attribute determines persistence and replication of actor state:
    ///     - Persisted: State is written to disk and replicated.
    ///     - Volatile: State is kept in memory only and replicated.
    ///     - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Volatile)]
    internal class FootballStatsActor : Actor, IFootballStatsActor
    {
        private readonly ITeamStatsRepository _teamStatsRepository;

        /// <summary>
        ///     Initializes a new instance of FootballStatsActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        /// <param name="teamStatsRepository">The repository backing the actor.</param>
        public FootballStatsActor(
            ActorService actorService, 
            ActorId actorId, 
            ITeamStatsRepository teamStatsRepository)
            : base(actorService, actorId)
        {
            _teamStatsRepository = teamStatsRepository;
        }

        /// <summary>
        ///     This method is called whenever an actor is activated.
        ///     An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            //return this.StateManager.TryAddStateAsync("count", 0);
           
        }

        public async Task<TeamStatsDto> Get()
        {
            var persistedDto = await StateManager.TryGetStateAsync<TeamStatsDto>(GetActorId());

            if (persistedDto.HasValue)
            {
                return persistedDto.Value;
            }
            
            var idSegments = Id.GetStringId().Split('/');

            var dto = await _teamStatsRepository.GetTeamStatsAsync(
                idSegments[0],
                short.Parse(idSegments[1]),
                byte.Parse(idSegments[2]));

            await StateManager.SetStateAsync(GetActorId(), new TeamStatsDto
            {
                Year = dto.Year,
                TeamId = dto.TeamId,
                TeamName = dto.TeamName,
                GamesPlayed = dto.GamesPlayed,
                Losses = dto.Losses,
                Wins = dto.Wins,
                PointsFor = dto.PointsFor,
                PointsAgainst = dto.PointsAgainst,
                Week = dto.Week
            });

            await StateManager.SaveStateAsync();

            return dto;
        }

        public async Task Update(TeamStatsDto dto)
        {
            await _teamStatsRepository.UpsertTeamStatsAsync(dto);

            await StateManager.RemoveStateAsync(GetActorId());

            await StateManager.SetStateAsync(GetActorId(), new TeamStatsDto
            {
                Year = dto.Year,
                TeamId = dto.TeamId,
                TeamName = dto.TeamName,
                GamesPlayed = dto.GamesPlayed,
                Losses = dto.Losses,
                Wins = dto.Wins,
                PointsFor = dto.PointsFor,
                PointsAgainst = dto.PointsAgainst,
                Week = dto.Week
            });

            await StateManager.SaveStateAsync();
        }

        private string GetActorId()
        {
            return Id.ToString().ToUpperInvariant();
        }
    }
}