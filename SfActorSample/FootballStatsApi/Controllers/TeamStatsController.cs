using System.Threading.Tasks;
using AutoMapper;
using FootballStatsApi.Common.Contracts;
using FootballStatsApi.Dal.Common.Dto;
using FootballStatsApi.Dal.Common.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FootballStatsApi.Controllers
{
    [Route("api/v1/stats/teams")]
    public class TeamStatsController : Controller
    {
        private readonly ITeamStatsRepository _teamStatsRepository;
        private readonly IMapper _mapper;

        public TeamStatsController(
            ITeamStatsRepository teamStatsRepository,
            IMapper mapper)
        {
            _teamStatsRepository = teamStatsRepository;
            _mapper = mapper;
        }

        [HttpGet("{id}/{year}/{week}", Name = "GetTeamStats")]
        [SwaggerResponse(200, typeof(GetTeamStatsResponse),
            "The operation was successful. The response contains statistics for the specified team, year, and week.")]
        [SwaggerResponse(404, Description = "The statistics for the specified team, year, and week was not found.")]
        public async Task<IActionResult> GetTeamStats(GetTeamStatsRequest request)
        {
            var dto = await _teamStatsRepository.GetTeamStatsAsync(request.Id, request.Year, request.Week);
            var responseModel = _mapper.Map<GetTeamStatsResponse>(dto);

            if (responseModel == null)
            {
                return NotFound();
            }

            return Ok(responseModel);
        }

        [HttpPut]
        [SwaggerResponse(201, Description =
            "The operation was successful. The response contains the object. The location header contains the address of the object.")]
        public async Task<IActionResult> PutTeamStats([FromBody] PutTeamStatsRequest request)
        {
            var dto = _mapper.Map<TeamStatsDto>(request);
            await _teamStatsRepository.UpsertTeamStatsAsync(dto);

            return CreatedAtRoute(
                "GetTeamStats",
                new {id = request.TeamId, year = request.Year, week = request.Week},
                request);
        }
    }
}