using System.Threading.Tasks;
using FootballStatsApi.Dal.Common.Dto;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly:
    FabricTransportActorRemotingProvider(RemotingListener = RemotingListener.V2Listener,
        RemotingClient = RemotingClient.V2Client)]

namespace FootballStatsActor.Interfaces
{
    /// <summary>
    ///     This interface defines the methods exposed by an actor.
    ///     Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IFootballStatsActor : IActor
    {
        Task<TeamStatsDto> Get();
        Task Update(TeamStatsDto dto);
    }
}