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
    public class DeleteModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(FuneralOfficeSystem.Data.ApplicationDbContext context, ILogger<DeleteModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public BurialPlace BurialPlace { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια απενεργοποίησης κοιμητηρίου χωρίς καθορισμένο ID");
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια απενεργοποίησης κοιμητηρίου χωρίς καθορισμένο ID");
                return NotFound();
            }

            try
            {
                var burialPlace = await _context.BurialPlaces
                    .Include(c => c.Funerals)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (burialPlace != null)
                {
                    _logger.LogInformation($"Απενεργοποίηση κοιμητηρίου με ID {id}");

                    // Αντί για διαγραφή, θέτουμε το IsEnabled σε false
                    burialPlace.IsEnabled = false;
                    _context.Attach(burialPlace).State = EntityState.Modified;
                    _logger.LogInformation("Απενεργοποίηση κοιμητηρίου αντί για διαγραφή");

                    await _context.SaveChangesAsync();
                }
                else
                {
                    _logger.LogWarning($"Δεν βρέθηκε κοιμητήριο με ID {id} κατά την απενεργοποίηση");
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την απενεργοποίηση κοιμητηρίου");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Page();
            }
        }
    }
}