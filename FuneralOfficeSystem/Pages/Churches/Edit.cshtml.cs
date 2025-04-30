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

namespace FuneralOfficeSystem.Pages.Churches
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
        public Church Church { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια επεξεργασίας εκκλησίας χωρίς καθορισμένο ID");
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

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync της Edit εκτελείται");
            _logger.LogInformation($"Church ID: {Church?.Id}");
            _logger.LogInformation($"Church Name: {Church?.Name}");
            _logger.LogInformation($"Church Address: {Church?.Address}");
            _logger.LogInformation($"Church Phone: {Church?.Phone}");
            _logger.LogInformation($"Church IsEnabled: {Church?.IsEnabled}");

            // Έλεγχος αν το μοντέλο είναι null
            if (Church == null)
            {
                _logger.LogWarning("Church είναι null");
                ModelState.AddModelError("", "Η φόρμα δεν έστειλε δεδομένα");
                return Page();
            }

            // Έλεγχος για υποχρεωτικά πεδία
            if (string.IsNullOrWhiteSpace(Church.Name))
            {
                ModelState.AddModelError("Church.Name", "Το όνομα είναι υποχρεωτικό");
            }

            if (string.IsNullOrWhiteSpace(Church.Address))
            {
                ModelState.AddModelError("Church.Address", "Η διεύθυνση είναι υποχρεωτική");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Get the existing church to preserve navigation properties
                var existingChurch = await _context.Churches
                    .Include(c => c.Funerals)
                    .FirstOrDefaultAsync(m => m.Id == Church.Id);

                if (existingChurch == null)
                {
                    return NotFound();
                }

                // Update properties
                existingChurch.Name = Church.Name;
                existingChurch.Address = Church.Address;
                existingChurch.Phone = Church.Phone;
                existingChurch.IsEnabled = Church.IsEnabled;

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
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η ενημέρωση της εκκλησίας.");
                    return Page();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Σφάλμα συγχρονισμού κατά την ενημέρωση της εκκλησίας");
                if (!ChurchExists(Church.Id))
                {
                    _logger.LogWarning($"Δεν βρέθηκε εκκλησία με ID {Church.Id}");
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
                _logger.LogError(ex, "Σφάλμα κατά την ενημέρωση της εκκλησίας");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Page();
            }
        }

        private bool ChurchExists(int id)
        {
            return _context.Churches.Any(e => e.Id == id);
        }
    }
}