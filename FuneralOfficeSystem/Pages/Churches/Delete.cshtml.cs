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
        public Church Church { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια απενεργοποίησης εκκλησίας χωρίς καθορισμένο ID");
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια απενεργοποίησης εκκλησίας χωρίς καθορισμένο ID");
                return NotFound();
            }

            try
            {
                var church = await _context.Churches
                    .Include(c => c.Funerals)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (church != null)
                {
                    _logger.LogInformation($"Απενεργοποίηση εκκλησίας με ID {id}");

                    // Αντί για διαγραφή, θέτουμε το IsEnabled σε false
                    church.IsEnabled = false;
                    _context.Attach(church).State = EntityState.Modified;
                    _logger.LogInformation("Απενεργοποίηση εκκλησίας αντί για διαγραφή");

                    await _context.SaveChangesAsync();
                }
                else
                {
                    _logger.LogWarning($"Δεν βρέθηκε εκκλησία με ID {id} κατά την απενεργοποίηση");
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την απενεργοποίηση εκκλησίας");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Page();
            }
        }
    }
}