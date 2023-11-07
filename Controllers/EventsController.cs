using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAPIASPNET.Contexts;
using OpenAPIASPNET.Contexts.Models;

namespace OpenAPIASPNET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly OpenAPIASPNETContext _context;

        public EventsController(OpenAPIASPNETContext context)
        {
            _context = context;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Events>>> GetEvents()
        {
            if (_context.Events == null)
            {
                return NotFound();
            }
            return await _context.Events.ToListAsync();
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Events>> GetEvents(Guid id)
        {
            if (_context.Events == null)
            {
                return NotFound();
            }
            var events = await _context.Events.FindAsync(id);

            if (events == null)
            {
                return NotFound();
            }

            return events;
        }

        // POST: api/Events
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Events>> PostEvents(Events events)
        {
            if (_context.Events == null)
            {
                return Problem("Entity set 'OpenAPIASPNETContext.Events'  is null.");
            }
            _context.Events.Add(events);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvents", new { id = events.Id }, events);
        }

        [HttpPost("rabbit")]
        public async Task<ActionResult<Events>> PushFromRabbit()
        {
            

            return CreatedAtAction("", null);
        }

        private bool EventsExists(Guid id)
        {
            return (_context.Events?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
