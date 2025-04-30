using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;
using Microsoft.Extensions.Logging;

namespace FuneralOfficeSystem.Pages.Churches
{
    public class DetailsModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(FuneralOfficeSystem.Data.ApplicationDbContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Church Church { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια προβολής λεπτομερειών εκκλησίας χωρίς καθορισμένο ID");
                return NotFound();
            }

            var church = await _context.Churches
                .Include(c => c.Funerals)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (church == null)
            {
                _logger.LogWarning($"Δεν βρέθηκε εκκλησία με ID {id}");
                return NotFound();
            }

            Church = church;
            return Page();
        }
    }
}