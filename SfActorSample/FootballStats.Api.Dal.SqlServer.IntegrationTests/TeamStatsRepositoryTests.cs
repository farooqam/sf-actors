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
        public async Task GetTeamStatsAsync_When_Stats_Not_Found_Returns_Null()
        {
            var result = await _repository.GetTeamStatsAsync("foo", 1999, 10);
            result.Should().BeNull();
        }

        [Test]
        public async Task GetTeamStatsAsync_GetsTheTeamStats()
        {
            var expectedDto = new TeamStatsDto
            {
                Year = 2017,
                Week = 10,
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
                actualDto = await _repository.GetTeamStatsAsync(expectedDto.TeamId, expectedDto.Year, expectedDto.Week);
            }

            actualDto.TeamId.Should().Be(expectedDto.TeamId);
            actualDto.TeamName.Should().Be(expectedDto.TeamName);
            actualDto.Year.Should().Be(expectedDto.Year);
            actualDto.Week.Should().Be(expectedDto.Week);
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
                Week = 10,
                TeamId = "NYG",
                TeamName = "New York Giants",
                GamesPlayed = 10,
                Wins = 2,
                Losses = 8,
                PointsFor = 188,
                PointsAgainst = 202
            };

            TeamStatsDto actualDto = null;

            var updatedDto = new TeamStatsDto
            {
                Year = 2017,
                Week = 12,
                TeamId = "NYG",
                TeamName = "New York Giants",
                GamesPlayed = 12,
                Wins = 3,
                Losses = 9,
                PointsFor = 202,
                PointsAgainst = 226
            };

            using (new TransactionScope(TransactionScopeOption.RequiresNew,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                await _repository.UpsertTeamStatsAsync(originalDto);
                await _repository.UpsertTeamStatsAsync(updatedDto);
                actualDto = await _repository.GetTeamStatsAsync(originalDto.TeamId, originalDto.Year, updatedDto.Week);
            }

            actualDto.Year.Should().Be(originalDto.Year);
            actualDto.Week.Should().Be(updatedDto.Week);
            actualDto.TeamId.Should().Be(originalDto.TeamId);
            actualDto.TeamName.Should().Be(originalDto.TeamName);
            actualDto.GamesPlayed.Should().Be(updatedDto.GamesPlayed);
            actualDto.Wins.Should().Be(updatedDto.Wins);
            actualDto.Losses.Should().Be(updatedDto.Losses);
            actualDto.PointsFor.Should().Be(updatedDto.PointsFor);
            actualDto.PointsAgainst.Should().Be(updatedDto.PointsAgainst);
        }

        [Test]
        public async Task UpsertTeamStatsAsync_Duplicate_Year_Week_TeamId_Test()
        {
            var dto = new TeamStatsDto
            {
                Year = 2017,
                Week = 10,
                TeamId = "NYG",
                TeamName = "New York Giants",
                GamesPlayed = 10,
                Wins = 2,
                Losses = 8,
                PointsFor = 188,
                PointsAgainst = 202
            };

            TeamStatsDto actualDto = null;

            using (new TransactionScope(TransactionScopeOption.RequiresNew,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                await _repository.UpsertTeamStatsAsync(dto);
                await _repository.UpsertTeamStatsAsync(dto);

                actualDto = await _repository.GetTeamStatsAsync(dto.TeamId, dto.Year, dto.Week);
            }

            actualDto.Should().NotBeNull();
        }
    }
}