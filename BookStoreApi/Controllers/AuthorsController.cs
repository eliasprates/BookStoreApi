using BookStoreApi.Data;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly BookStoreContext _context;

        public AuthorsController(BookStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            return await _context.Authors.Include(a => a.Books).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await _context.Authors.Include(a => a.Books)
                                               .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(Author author)
        {
            if (string.IsNullOrWhiteSpace(author.Name))
            {
                return BadRequest();
            }

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            _context.Entry(author).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
