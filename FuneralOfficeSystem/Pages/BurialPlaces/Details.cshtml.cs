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

namespace FuneralOfficeSystem.Pages.BurialPlaces
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

        public BurialPlace BurialPlace { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια προβολής λεπτομερειών κοιμητηρίου χωρίς καθορισμένο ID");
                return NotFound();
            }

            var burialPlace = await _context.BurialPlaces
                .Include(c => c.Funerals)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (burialPlace == null)
            {
                _logger.LogWarning($"Δεν βρέθηκε κοιμητήριο με ID {id}");
                return NotFound();
            }

            BurialPlace = burialPlace;
            return Page();
        }
    }
}