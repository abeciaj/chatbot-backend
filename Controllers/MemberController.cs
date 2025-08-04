using ChatSupport.Data;
using ChatSupport.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatSupport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly UserDbContext _context;

        public MemberController(UserDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Member>>> GetMembers()
        {
            var members = await _context.members
                .Include(m => m.User)
                .Include(m => m.Room)
                .ToListAsync();
            return Ok(members);
        }

        [HttpGet("{member_id}")]
        public async Task<ActionResult<Member>> GetMemberById(long member_id)
        {
            var member = await _context.members
                .Include(m => m.User)
                .Include(m => m.Room)
                .FirstOrDefaultAsync(m => m.member_id == member_id);

            if (member is null)
                return NotFound();

            return Ok(member);
        }

        [HttpGet("room/{room_id}")]
        public async Task<ActionResult<List<Member>>> GetMembersByRoom(long room_id)
        {
            var members = await _context.members
                .Include(m => m.User)
                .Where(m => m.room_id == room_id)
                .ToListAsync();

            if (members is null || members.Count == 0)
                return NotFound();

            return Ok(members);
        }

        [HttpPost]
        public async Task<ActionResult<Member>> AddMember(Member newMember)
        {
            if (newMember is null)
                return BadRequest();

            _context.members.Add(newMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMemberById), new { member_id = newMember.member_id }, newMember);
        }

        [HttpPut("{member_id}")]
        public async Task<IActionResult> UpdateMember(long member_id, Member updatedMember)
        {
            var member = await _context.members.FindAsync(member_id);
            if (member is null)
                return NotFound();

            member.user_id = updatedMember.user_id;
            member.room_id = updatedMember.room_id;
            member.role = updatedMember.role;
            member.joined_on = updatedMember.joined_on;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{member_id}")]
        public async Task<IActionResult> DeleteMember(long member_id)
        {
            var member = await _context.members.FindAsync(member_id);
            if (member is null)
                return NotFound();

            _context.members.Remove(member);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("user/{user_id}")]
        public async Task<ActionResult<List<Room>>> GetRoomsByUser(long user_id)
        {
            var rooms = await _context.members
                .Include(m => m.Room)
                .Where(m => m.user_id == user_id)
                .Select(m => m.Room)
                .ToListAsync();

            return Ok(rooms);
        }
    }
}
