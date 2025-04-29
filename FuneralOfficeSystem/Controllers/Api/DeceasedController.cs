using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;

namespace FuneralOfficeSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeceasedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DeceasedController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return Ok(new List<object>());

            var deceased = await _context.Deceased
                .Where(d => d.FirstName.Contains(searchTerm) ||
                           d.LastName.Contains(searchTerm))
                .Select(d => new {
                    id = d.Id,
                    name = d.FirstName + " " + d.LastName
                })
                .Take(10)
                .ToListAsync();

            return Ok(deceased);
        }
    }
}