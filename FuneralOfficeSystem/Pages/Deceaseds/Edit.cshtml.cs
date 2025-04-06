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

namespace FuneralOfficeSystem.Pages.Deceaseds
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
        public Deceased Deceased { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deceased = await _context.Deceased.FirstOrDefaultAsync(m => m.Id == id);
            if (deceased == null)
            {
                return NotFound();
            }
            Deceased = deceased;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync της Edit εκτελείται");
            _logger.LogInformation($"Deceased ID: {Deceased?.Id}");
            _logger.LogInformation($"Deceased FirstName: {Deceased?.FirstName ?? "NULL"}");
            _logger.LogInformation($"Deceased LastName: {Deceased?.LastName ?? "NULL"}");
            _logger.LogInformation($"Deceased DeathDate: {Deceased?.DeathDate.ToString() ?? "NULL"}");

            // Έλεγχος αν το μοντέλο είναι null
            if (Deceased == null)
            {
                _logger.LogWarning("Deceased είναι null");
                ModelState.AddModelError("", "Η φόρμα δεν έστειλε δεδομένα");
                return Page();
            }

            // Καθαρισμός του ModelState για να αποφύγουμε προβλήματα με validation
            ModelState.Clear();

            try
            {
                _logger.LogInformation("Προσπάθεια ενημέρωσης Deceased");
                _context.Attach(Deceased).State = EntityState.Modified;
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
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η ενημέρωση του αποβιώσαντα.");
                    return Page();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Σφάλμα συγχρονισμού κατά την ενημέρωση του αποβιώσαντα");
                if (!DeceasedExists(Deceased.Id))
                {
                    _logger.LogWarning($"Δεν βρέθηκε αποβιώσαντας με ID {Deceased.Id}");
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
                _logger.LogError(ex, "Σφάλμα κατά την ενημέρωση του αποβιώσαντα");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Page();
            }
        }

        private bool DeceasedExists(int id)
        {
            return _context.Deceased.Any(e => e.Id == id);
        }
    }
}