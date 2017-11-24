using System;
using System.Threading.Tasks;
using System.Transactions;
using FluentAssertions;
using FootballStatsApi.Dal.Common.Dto;
using FootballStatsApi.Dal.Common.Repositories;
using FootballStatsApi.Dal.SqlServer.Repositories;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace FootballStats.Api.Dal.SqlServer.IntegrationTests
{
    [TestFixture]
    public class TeamStatsRepositoryTests
    {
        private ITeamStatsRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true);

            var configuration = builder.Build();

            var configSection = configuration.GetSection("FootballStatsApi.Dal.SqlServer");

            var settings = new TeamStatsRepositorySettings
            {
                ConnectionString = configSection["connectionString"],
                QueryTimeout = TimeSpan.Parse(configSection["queryTimeout"])
            };

            _repository = new TeamStatsRepository(settings);
        }

        [Test]
        public async Task GetTeamStatsAsync_GetsTheTeamStats()
        {
            var expectedDto = new TeamStatsDto
            {
                Year = 2017,
                TeamId = "NYG",
                TeamName = "New York Giants",
                GamesPlayed = 10,
                Wins = 2,
                Losses = 8,
                PointsFor = 188,
                PointsAgainst = 202
            };

            TeamStatsDto actualDto;

            using (new TransactionScope(TransactionScopeOption.RequiresNew,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                await _repository.UpsertTeamStatsAsync(expectedDto);
                actualDto = await _repository.GetTeamStatsAsync(expectedDto.TeamId);
            }

            actualDto.TeamId.Should().Be(expectedDto.TeamId);
            actualDto.TeamName.Should().Be(expectedDto.TeamName);
            actualDto.Year.Should().Be(expectedDto.Year);
            actualDto.GamesPlayed.Should().Be(expectedDto.GamesPlayed);
            actualDto.Wins.Should().Be(expectedDto.Wins);
            actualDto.Losses.Should().Be(expectedDto.Losses);
            actualDto.PointsFor.Should().Be(expectedDto.PointsFor);
            actualDto.PointsAgainst.Should().Be(expectedDto.PointsAgainst);
        }

        [Test]
        public async Task UpsertTeamStatsAsync_UpdatesTheTeamStats()
        {
            var originalDto = new TeamStatsDto
            {
                Year = 2017,
                TeamId = "NYG",
                TeamName = "New York Giants",
                GamesPlayed = 10,
                Wins = 2,
                Losses = 8,
                PointsFor = 188,
                PointsAgainst = 202
            };

            TeamStatsDto actualDto = null;

            byte expectedGamesPlayed = 11;

            using (new TransactionScope(TransactionScopeOption.RequiresNew,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                await _repository.UpsertTeamStatsAsync(originalDto);
                originalDto.GamesPlayed = expectedGamesPlayed;
                await _repository.UpsertTeamStatsAsync(originalDto);
                actualDto = await _repository.GetTeamStatsAsync(originalDto.TeamId);
            }

            actualDto.GamesPlayed.Should().Be(expectedGamesPlayed);
        }
    }
}