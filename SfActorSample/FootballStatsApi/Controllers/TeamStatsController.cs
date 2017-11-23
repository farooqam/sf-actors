using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FootballStatsApi.Common.Contracts;
using FootballStatsApi.Dal.Common.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FootballStatsApi.Controllers
{
    [Route("api/stats/teams")]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamStats(string id)
        {
            var dtos = await _teamStatsRepository.GetTeamStatsAsync(id);
            var reponseModels = _mapper.Map<IEnumerable<GetTeamStatsResponse>>(dtos);
            return Ok(reponseModels);
        }
    }
}