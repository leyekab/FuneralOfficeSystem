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

namespace FuneralOfficeSystem.Pages.BurialPlaces
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
        public BurialPlace BurialPlace { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια επεξεργασίας κοιμητηρίου χωρίς καθορισμένο ID");
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

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync της Edit εκτελείται");
            _logger.LogInformation($"BurialPlace ID: {BurialPlace?.Id}");
            _logger.LogInformation($"BurialPlace Name: {BurialPlace?.Name}");
            _logger.LogInformation($"BurialPlace Address: {BurialPlace?.Address}");
            _logger.LogInformation($"BurialPlace Phone: {BurialPlace?.Phone}");
            _logger.LogInformation($"BurialPlace IsEnabled: {BurialPlace?.IsEnabled}");

            // Έλεγχος αν το μοντέλο είναι null
            if (BurialPlace == null)
            {
                _logger.LogWarning("BurialPlace είναι null");
                ModelState.AddModelError("", "Η φόρμα δεν έστειλε δεδομένα");
                return Page();
            }

            // Έλεγχος για υποχρεωτικά πεδία
            if (string.IsNullOrWhiteSpace(BurialPlace.Name))
            {
                ModelState.AddModelError("BurialPlace.Name", "Το όνομα είναι υποχρεωτικό");
            }

            if (string.IsNullOrWhiteSpace(BurialPlace.Address))
            {
                ModelState.AddModelError("BurialPlace.Address", "Η διεύθυνση είναι υποχρεωτική");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Get the existing burial place to preserve navigation properties
                var existingBurialPlace = await _context.BurialPlaces
                    .Include(c => c.Funerals)
                    .FirstOrDefaultAsync(m => m.Id == BurialPlace.Id);

                if (existingBurialPlace == null)
                {
                    return NotFound();
                }

                // Update properties
                existingBurialPlace.Name = BurialPlace.Name;
                existingBurialPlace.Address = BurialPlace.Address;
                existingBurialPlace.Phone = BurialPlace.Phone;
                existingBurialPlace.IsEnabled = BurialPlace.IsEnabled;

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
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η ενημέρωση του κοιμητηρίου.");
                    return Page();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Σφάλμα συγχρονισμού κατά την ενημέρωση του κοιμητηρίου");
                if (!BurialPlaceExists(BurialPlace.Id))
                {
                    _logger.LogWarning($"Δεν βρέθηκε κοιμητήριο με ID {BurialPlace.Id}");
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
                _logger.LogError(ex, "Σφάλμα κατά την ενημέρωση του κοιμητηρίου");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Page();
            }
        }

        private bool BurialPlaceExists(int id)
        {
            return _context.BurialPlaces.Any(e => e.Id == id);
        }
    }
}