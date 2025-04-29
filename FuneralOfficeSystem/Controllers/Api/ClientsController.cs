using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FuneralOfficeSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Ok(new object[] { });

            var clients = await _context.Clients
                .Where(d => d.FirstName.Contains(searchTerm) ||
                           d.LastName.Contains(searchTerm))
                .Select(d => new {
                    id = d.Id,
                    name = d.FirstName + " " + d.LastName
                })
                .Take(10)
                .ToListAsync();

            return Ok(clients);
        }
    }
}