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
    public class BurialPlacesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BurialPlacesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BurialPlaces/Search
        [HttpGet("Search")]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Ok(Array.Empty<object>());
            }

            var burialPlaces = await _context.BurialPlaces
                .Where(b => b.IsEnabled &&
                           (b.Name.Contains(searchTerm) ||
                            b.Address.Contains(searchTerm)))
                .Select(b => new
                {
                    id = b.Id,
                    name = $"{b.Name} - {b.Address}"
                })
                .Take(10)
                .ToListAsync();

            return Ok(burialPlaces);
        }
    }
}