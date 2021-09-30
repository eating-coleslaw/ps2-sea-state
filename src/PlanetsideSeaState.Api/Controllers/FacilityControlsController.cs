using Microsoft.AspNetCore.Mvc;
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

        // GET: api/<FacilityControlController>
        [HttpGet("recent")]
        public async Task<IEnumerable<FacilityControlInfo>> GetRecentFacilityControlsAsync([FromQuery] short? worldId, [FromQuery] int? facilityId, [FromQuery] short? limit)
        {
            return await _eventRepository.GetRecentFacilityControlsAsync(worldId, facilityId, limit);
        }

        // GET api/<FacilityControlController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FacilityControlController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<FacilityControlController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FacilityControlController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
