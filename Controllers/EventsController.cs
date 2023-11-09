using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAPIASPNET.Contexts;
using OpenAPIASPNET.Contexts.Models;

namespace OpenAPIASPNET.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly OpenAPIASPNETContext _context;
        private readonly IBus consumer;

        public EventsController(OpenAPIASPNETContext context, IBus message_queue)
        {
            _context = context;
            consumer = message_queue;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Events>>> GetEvents()
        {
            if (_context.Events == null)
            {
                return NotFound();
            }
            return await _context.Events.ToListAsync();
        }

        // GET: api/events/00000000-0000-0000-0000-000000000000
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

        // POST: api/events/manual
        [HttpPost("manual")]
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

        // POST: api/events/rabbit
        [HttpPost("rabbit")]
        public async Task<ActionResult<Events>> GatherEvents()
        {
            try
            {
                using (consumer.SendReceive.Receive<Events>("events", (events) =>
                {
                    _context.Events.Add(events);
                    _context.SaveChanges();
                })) ;
            }
            catch (Exception ex)
            {
                return Problem($"Exception while retriving messages from queue: {ex.Message}");
            }
            return CreatedAtAction("GetEvents", null);
    }


    private bool EventsExists(Guid id)
    {
        return (_context.Events?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
}
