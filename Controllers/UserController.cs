using ChatSupport.Data;
using ChatSupport.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatSupport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserDbContext context) : ControllerBase
    {
        private readonly UserDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return Ok(await _context.users.ToListAsync());
        }
        [HttpGet("{user_id}")]
        public async Task<ActionResult<User>> GetUserById(long user_id)
        {
            var user = await _context.users.FindAsync(user_id);
            if (user is null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> AddUser(User newUser)
        {
            if (newUser is null)
                return BadRequest();

            _context.users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { user_id = newUser.user_id }, newUser);

        }

        [HttpPut("{user_id}")]
        public async Task<IActionResult> UpdateUser(long user_id, User updatedUser)
        {
            var user = await _context.users.FindAsync(user_id);
            if (user is null)
                return NotFound();

            user.name = updatedUser.name;
            user.email = updatedUser.email;
            user.password_hash = updatedUser.password_hash;
            user.created_on = updatedUser.created_on;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{user_id}")]
        public async Task<IActionResult> DeleteUser(long user_id)
        {
            var user = await _context.users.FindAsync(user_id);
            if (user is null)
                return NotFound();

            _context.users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    };

    
}
