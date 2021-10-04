using Microsoft.AspNetCore.Mvc;
using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Models.QueryResults;
using PlanetsideSeaState.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlanetsideSeaState.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityControlsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public FacilityControlsController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        // GET: api/FacilityControls/recent?worldId=17&facilityId=224&limit=10
        [HttpGet("recent")]
        public async Task<IEnumerable<FacilityControlInfo>> GetRecentFacilityControlsAsync([FromQuery] short? worldId, [FromQuery] int? facilityId, [FromQuery] short? limit)
        {
            return await _eventRepository.GetRecentFacilityControlsAsync(worldId, facilityId, limit);
        }

        // GET api/<FacilityControlController>/c9c71fcc-435b-46b0-b840-10b196915a17
        [HttpGet("{id}")]
        public async Task<FacilityControl> GetFacilityControlsWithAttributedPlayers(Guid id)
        {
            return await _eventRepository.GetFacilityControlWithAttributedPlayers(id);
        }

        // GET api/<FacilityControlController>/players/c9c71fcc-435b-46b0-b840-10b196915a17
        [HttpGet("players/{id}")]
        public async Task<IEnumerable<PlayerFacilityControl>> GetFacilityControlsAttributedPlayers(Guid id)
        {
            return await _eventRepository.GetFacilityControlAttributedPlayers(id);
        }

        // DELETE api/<FacilityControlController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
