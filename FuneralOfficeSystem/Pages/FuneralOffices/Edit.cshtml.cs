using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;
using Microsoft.Extensions.Logging;

namespace FuneralOfficeSystem.Pages.FuneralOffices
{
    public class EditModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;
        private readonly ILogger<EditModel> _logger;

        public EditModel(FuneralOfficeSystem.Data.ApplicationDbContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public FuneralOffice FuneralOffice { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funeraloffice = await _context.FuneralOffices.FirstOrDefaultAsync(m => m.Id == id);
            if (funeraloffice == null)
            {
                return NotFound();
            }
            FuneralOffice = funeraloffice;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync της Edit εκτελείται");
            _logger.LogInformation($"FuneralOffice ID: {FuneralOffice?.Id}");
            _logger.LogInformation($"FuneralOffice Name: {FuneralOffice?.Name ?? "NULL"}");
            _logger.LogInformation($"FuneralOffice Address: {FuneralOffice?.Address ?? "NULL"}");
            _logger.LogInformation($"FuneralOffice Phone: {FuneralOffice?.Phone ?? "NULL"}");
            _logger.LogInformation($"IsEnabled: {FuneralOffice?.IsEnabled}");

            // Έλεγχος αν το μοντέλο είναι null
            if (FuneralOffice == null)
            {
                _logger.LogWarning("FuneralOffice είναι null");
                ModelState.AddModelError("", "Η φόρμα δεν έστειλε δεδομένα");
                return Page();
            }

            // Καθαρισμός του ModelState για να αποφύγουμε προβλήματα με validation
            ModelState.Clear();

            try
            {
                _logger.LogInformation("Προσπάθεια ενημέρωσης FuneralOffice");
                _context.Attach(FuneralOffice).State = EntityState.Modified;
                _logger.LogInformation("Προσπάθεια αποθήκευσης αλλαγών");
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation($"Αποτέλεσμα αποθήκευσης: {result} εγγραφές αποθηκεύτηκαν");

                if (result > 0)
                {
                    _logger.LogInformation("Επιτυχής ενημέρωση, ανακατεύθυνση στο Index");
                    return RedirectToPage("./Index");
                }
                else
                {
                    _logger.LogWarning("Δεν αποθηκεύτηκαν αλλαγές");
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η ενημέρωση του γραφείου τελετών.");
                    return Page();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Σφάλμα συγχρονισμού κατά την ενημέρωση του γραφείου τελετών");
                if (!FuneralOfficeExists(FuneralOffice.Id))
                {
                    _logger.LogWarning($"Δεν βρέθηκε γραφείο τελετών με ID {FuneralOffice.Id}");
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", $"Προέκυψε σφάλμα συγχρονισμού: {ex.Message}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την ενημέρωση του γραφείου τελετών");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Page();
            }
        }

        private bool FuneralOfficeExists(int id)
        {
            return _context.FuneralOffices.Any(e => e.Id == id);
        }
    }
}