using Microsoft.AspNetCore.Mvc;
using WeeklyPlanner.Model.Entities;
using WeeklyPlanner.Model.Repositories;
using Microsoft.AspNetCore.Authorization;


namespace WeeklyPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomieController : ControllerBase
    {
        private readonly RoomieRepository _roomieRepository;

        public RoomieController(RoomieRepository roomieRepository)
        {
            _roomieRepository = roomieRepository;
        }

        // GET: api/roomie/{loginid}
        [AllowAnonymous]
        [HttpGet("{loginid}")]
        public ActionResult<IEnumerable<Roomie>> GetRoomiesByLoginId([FromRoute] int loginid)
        {
            try
            {
                var roomies = _roomieRepository.GetRoomiesByLoginId(loginid);
                return Ok(roomies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/roomie
        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddRoomie([FromBody] Roomie roomie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = _roomieRepository.AddRoomie(roomie);
                if (result)
                {
                    return Ok("Roomie added successfully.");
                }

                return BadRequest("Failed to add roomie.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/roomie/{roomieid}
        [AllowAnonymous]
        [HttpDelete("{roomieid}")]
        public ActionResult DeleteRoomie([FromRoute] int roomieid)
        {
            try
            {
                var result = _roomieRepository.DeleteRoomie(roomieid);
                if (result)
                {
                    return NoContent();
                }

                return BadRequest("Failed to delete roomie.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
