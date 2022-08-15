using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GuestBook.Models;
using System.Security.Claims;

namespace GuestBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly guestbookContext _context;
        User user;

        public MessagesController(guestbookContext context)
        {
            _context = context;
        }

        // GET: api/Messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
          if (_context.Messages == null)
          {
              return NotFound();
          }
            return await _context.Messages.ToListAsync();
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
          if (_context.Messages == null)
          {
              return NotFound();
          }
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }
        //edit
        // PUT: api/Messages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(int id, Message message)
        {
            user = GetCurrentUser();
            if (id != message.MsgId)
            {
                return BadRequest();
            }

            if (_context.Messages.Where(o => o.MsgId == id && o.SenderId == user.UserId) != null)
            {
                _context.Entry(message).State = EntityState.Modified;

            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        //write message
        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            user = GetCurrentUser();

            if (_context.Messages == null)
          {
              return Problem("Entity set 'guestbookContext.Messages'  is null.");
          }
            message.SenderId = user.UserId;
            message.ReplyMsgId = null;

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Created("anything", message);
        }
        //reply message
        [HttpPost("reply/{RepliedMsgID}")]
        public async Task<ActionResult<Message>> replyMessage(int RepliedMsgID,Message message)
        {
            user = GetCurrentUser();

            if (_context.Messages == null)
            {
                return Problem("Entity set 'guestbookContext.Messages'  is null.");
            }
            message.SenderId = user.UserId;
            message.ReplyMsgId = RepliedMsgID;

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Created("anything", message);
        }
        //delete message
        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            user = GetCurrentUser();
            if (_context.Messages == null)
            {
                return NotFound();
            }
            var message = await _context.Messages.Where(o=>o.SenderId==user.UserId&&o.MsgId==id).FirstOrDefaultAsync();
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(int id)
        {
            return (_context.Messages?.Any(e => e.MsgId == id)).GetValueOrDefault();
        }
        private User GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;// get identity of loggedin user

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new User
                {
                    UserId = int.Parse(userClaims.FirstOrDefault(o => o.Type == "ID")?.Value),
                    Email = userClaims.FirstOrDefault(o => o.Type == "Email")?.Value,
                    Password = userClaims.FirstOrDefault(o => o.Type == "Password")?.Value,
                };
            }
            return null;

        }
    }
}
