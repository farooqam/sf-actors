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

        [HttpGet("{id}", Name = "GetTeamStats")]
        [SwaggerResponse(200, typeof(GetTeamStatsResponse[]),
            "The operation was successful. The response contains an array of team statistics.")]
        public async Task<IActionResult> GetTeamStats(string id)
        {
            var dtos = await _teamStatsRepository.GetTeamStatsAsync(id);
            var responseModels = _mapper.Map<GetTeamStatsResponse>(dtos);

            if (responseModels == null)
            {
                return Ok();
            }

            return Ok(responseModels);
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
                new {id = request.TeamId},
                request);
        }
    }
}