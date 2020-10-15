using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuTNetCoreAPI.Body;
using TuTNetCoreAPI.DB;
using TuTNetCoreAPI.Models;

namespace TuTNetCoreAPI.Controllers
{
    // /api/Users
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DBContext _context;

        public UsersController(DBContext context)
        {
            _context = context;
        }


        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User
                .Where(user => user.Gender == Gender.Male)
                .ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // POST: api/Users/Drug
        [HttpPost("Drug")]
        public async Task<ActionResult<User>> BuyDrug(BuyDrug buyDrug)
        {
            var user = await _context.User.FindAsync(buyDrug.Id);
            if (user == null)
            {
                return NotFound();
            }

            List<Drug> drugList = new List<Drug>();
            for(var i = 0; i < buyDrug.Drugs.Length; i++)
            {
                var drug = await _context.Drug.FindAsync(buyDrug.Drugs[i]);
                if (drug == null)
                {
                    return NotFound();
                }

                drugList.Add(drug);
            }


            _context.User
                .Where(user => user.LastName == "A")
                .Where(user => user.FirstName == "Z")
                .Last();

            for (var i = 0; i < drugList.Count; i++)
            {
                var stores = _context.Drug
                        .Where<Drug>(drug => drug.Price > 100)
                        .OrderBy(drug => drug.Price)
                        .Single<Drug>();
             
                stores.Amount -= 1;
                _context.Drug.Update(stores);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, drugList);
        }


        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
