using System;
using System.Fabric;
using System.Threading;
using Autofac;
using Autofac.Integration.ServiceFabric;
using FootballStatsApi.Dal.Common.Repositories;
using FootballStatsApi.Dal.SqlServer.Repositories;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace FootballStatsActor
{
    internal static class Program
    {
        /// <summary>
        ///     This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            using (var container = BuildContainer())
            {
                try
                {
                    // This line registers an Actor Service to host your actor class with the Service Fabric runtime.
                    // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
                    // are automatically populated when you build this project.
                    // For more information, see https://aka.ms/servicefabricactorsplatform

                    //ActorRuntime.RegisterActorAsync<FootballStatsActor>(
                    //        (context, actorType) => new ActorService(
                    //            context, 
                    //            actorType, 
                    //            (service, actorId) => container.Resolve<FootballStatsActor>(
                    //                new TypedParameter(typeof(ActorService), service),
                    //                new TypedParameter(typeof(ActorId), actorId))))
                    //    .GetAwaiter()
                    //    .GetResult();

                    Thread.Sleep(Timeout.Infinite);
                }
                catch (Exception e)
                {
                    ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                    throw;
                }
            }
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            var package = FabricRuntime.GetActivationContext().GetConfigurationPackageObject("Config");
            var sqlServerRepository = new TeamStatsRepository(GetSqlServerSettings(package));

            builder.RegisterInstance(sqlServerRepository).As<ITeamStatsRepository>();
            builder.RegisterServiceFabricSupport();
            builder.RegisterActor<FootballStatsActor>();
            
            return builder.Build();
        }

        private static TeamStatsRepositorySettings GetSqlServerSettings(ConfigurationPackage package)
        {
            var configSection = package.Settings.Sections["FootballStatsApi.Dal.SqlServer"];
            var connectionString = configSection.Parameters["connectionString"].Value;
            var queryTimeout = TimeSpan.Parse(configSection.Parameters["queryTimeout"].Value);

            return new TeamStatsRepositorySettings
            {
                ConnectionString = connectionString,
                QueryTimeout = queryTimeout
            };
        }
    }
}