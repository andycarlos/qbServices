using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using qbService.Models;

namespace qbService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassChangesController : ControllerBase
    {
        /*private readonly ApplicationDbContex _context;

        public PassChangesController(ApplicationDbContex context)
        {
            _context = context;
        }

        // GET: api/PassChanges
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PassChange>>> GetPassChange()
        {
            return await _context.PassChange.ToListAsync();
        }

        // GET: api/PassChanges/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PassChange>> GetPassChange(string id)
        {
            var passChange = await _context.PassChange.FindAsync(id);

            if (passChange == null)
            {
                return NotFound();
            }

            return passChange;
        }

        // PUT: api/PassChanges/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPassChange(string id, PassChange passChange)
        {
            if (id != passChange.Id)
            {
                return BadRequest();
            }

            _context.Entry(passChange).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PassChangeExists(id))
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

        // POST: api/PassChanges
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<PassChange>> PostPassChange(PassChange passChange)
        {
            _context.PassChange.Add(passChange);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PassChangeExists(passChange.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPassChange", new { id = passChange.Id }, passChange);
        }

        // DELETE: api/PassChanges/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PassChange>> DeletePassChange(string id)
        {
            var passChange = await _context.PassChange.FindAsync(id);
            if (passChange == null)
            {
                return NotFound();
            }

            _context.PassChange.Remove(passChange);
            await _context.SaveChangesAsync();

            return passChange;
        }

        private bool PassChangeExists(string id)
        {
            return _context.PassChange.Any(e => e.Id == id);
        }*/
    }
}
