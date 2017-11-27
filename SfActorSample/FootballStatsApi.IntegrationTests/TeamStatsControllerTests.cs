using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FootballStatsApi.Common.Contracts;
using FootballStatsApi.Dal.Common.Dto;
using FootballStatsApi.Dal.Common.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FootballStatsApi.IntegrationTests
{
    [TestFixture]
    public class TeaamStatsControllerTests
    {
        private TestServer _server;
        private HttpClient _httpClient;
        private readonly Mock<ITeamStatsRepository> _mockRepository = new Mock<ITeamStatsRepository>();

        [SetUp]
        public void SetUp()
        {
            _server = new TestServer(
                new WebHostBuilder()
                    .UseStartup<Startup>()
                    .ConfigureServices(
                        collection => { collection.AddTransient(provider => _mockRepository.Object); }));

            _httpClient = _server.CreateClient();
        }

        [Test]
        public async Task Get_When_No_Team_Stats_Returns_Ok_With_No_Body()
        {
            _mockRepository.Setup(m => m.GetTeamStatsAsync(It.IsAny<string>(), It.IsAny<short>(), It.IsAny<byte>()))
                .ReturnsAsync(null as TeamStatsDto);

            var response = await _httpClient.GetAsync("api/v1/stats/teams/nyg/2017/10");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            (await response.Content.ReadAsStringAsync()).Should().BeEmpty();

        }

        [Test]
        public async Task Get_Returns_Team_Stats_With_Ok_Status_Code()
        {
            var expectedDto = new TeamStatsDto
            {
                TeamId = "NYG",
                Year = 2017,
                Week = 10,
                TeamName = "New York Giants",
                GamesPlayed = 10,
                Losses = 8,
                Wins = 2,
                PointsFor = 202,
                PointsAgainst = 213
            };

            _mockRepository.Setup(m => m.GetTeamStatsAsync(expectedDto.TeamId, expectedDto.Year, expectedDto.Week))
                .ReturnsAsync(expectedDto);

            var response = await _httpClient.GetAsync($"api/v1/stats/teams/{expectedDto.TeamId}/{expectedDto.Year}/{expectedDto.Week}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var actualDto =
                JsonConvert.DeserializeObject<GetTeamStatsResponse>(await response.Content.ReadAsStringAsync());

            actualDto.TeamId.Should().Be(expectedDto.TeamId);
            actualDto.TeamName.Should().Be(expectedDto.TeamName);
            actualDto.GamesPlayed.Should().Be(expectedDto.GamesPlayed);
            actualDto.Losses.Should().Be(expectedDto.Losses);
            actualDto.Wins.Should().Be(expectedDto.Wins);
            actualDto.PointsFor.Should().Be(expectedDto.PointsFor);
            actualDto.PointsAgainst.Should().Be(expectedDto.PointsAgainst);
            actualDto.Year.Should().Be(expectedDto.Year);
            actualDto.Week.Should().Be(expectedDto.Week);
        }

        [Test]
        public async Task Put_Returns_Created_Status_Code_And_Location_In_Header()
        {
            var expectedDto = new TeamStatsDto
            {
                TeamId = "foo"
            };

            _mockRepository.Setup(m => m.UpsertTeamStatsAsync(expectedDto)).Returns(Task.FromResult(0));

            var requestModel = new PutTeamStatsRequest
            {
                TeamId = expectedDto.TeamId
            };

            var response = await _httpClient.PutAsync("api/v1/stats/teams", Stringify(requestModel));
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().Be($"http://localhost/api/v1/stats/teams/{expectedDto.TeamId}/{expectedDto.Year}/{expectedDto.Week}");
            
        }

        private StringContent Stringify<T>(T obj) where T : class
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }
    }
}