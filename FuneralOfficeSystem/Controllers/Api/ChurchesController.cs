using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;

namespace FuneralOfficeSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChurchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChurchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Churches/Search
        [HttpGet("Search")]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Ok(Array.Empty<object>());
            }

            var churches = await _context.Churches
                .Where(c => c.IsEnabled &&
                           (c.Name.Contains(searchTerm) ||
                            c.Address.Contains(searchTerm)))
                .Select(c => new
                {
                    id = c.Id,
                    name = $"{c.Name} - {c.Address}"
                })
                .Take(10)
                .ToListAsync();

            return Ok(churches);
        }
    }
}