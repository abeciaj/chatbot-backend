using ChatSupport.Data;
using ChatSupport.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatSupport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController(UserDbContext context) : ControllerBase
    {
        private readonly UserDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<List<Room>>> Getrooms()
        {
            return Ok(await _context.rooms.ToListAsync());
        }
        [HttpGet("{room_id}")]
        public async Task<ActionResult<Room>> GetroomById(long room_id)
        {
            var room = await _context.rooms.FindAsync(room_id);
            if (room is null)
                return NotFound();

            return Ok(room);
        }

        [HttpPost]
        public async Task<ActionResult<Room>> Addroom(Room newroom)
        {
            if (newroom is null)
                return BadRequest();

            _context.rooms.Add(newroom);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetroomById), new { room_id = newroom.room_id }, newroom);

        }

        [HttpPut("{room_id}")]
        public async Task<IActionResult> Updateroom(long room_id, Room updatedroom)
        {
            var room = await _context.rooms.FindAsync(room_id);
            if (room is null)
                return NotFound();

            room.name = updatedroom.name;
            room.created_on = updatedroom.created_on;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{room_id}")]
        public async Task<IActionResult> Deleteroom(long room_id)
        {
            var room = await _context.rooms.FindAsync(room_id);
            if (room is null)
                return NotFound();

            _context.rooms.Remove(room);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    };


}
