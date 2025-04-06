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

namespace FuneralOfficeSystem.Pages.Funerals
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
        public Funeral Funeral { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια επεξεργασίας χωρίς καθορισμένο ID");
                return NotFound();
            }

            var funeral = await _context.Funerals.FirstOrDefaultAsync(m => m.Id == id);
            if (funeral == null)
            {
                _logger.LogWarning($"Δεν βρέθηκε κηδεία με ID {id}");
                return NotFound();
            }

            Funeral = funeral;

            // Ανανέωση των dropdown λιστών
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Address");
            ViewData["DeceasedId"] = new SelectList(_context.Deceased, "Id", "FirstName");
            ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices, "Id", "Address");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync της Edit εκτελείται");
            _logger.LogInformation($"Funeral ID: {Funeral?.Id}");
            _logger.LogInformation($"Funeral DeceasedId: {Funeral?.DeceasedId ?? 0}");
            _logger.LogInformation($"Funeral ClientId: {Funeral?.ClientId ?? 0}");
            _logger.LogInformation($"Funeral FuneralDate: {Funeral?.FuneralDate.ToString() ?? "NULL"}");

            // Έλεγχος αν το μοντέλο είναι null
            if (Funeral == null)
            {
                _logger.LogWarning("Funeral είναι null");
                ModelState.AddModelError("", "Η φόρμα δεν έστειλε δεδομένα");

                // Επαναφορά των dropdown λιστών
                ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Address");
                ViewData["DeceasedId"] = new SelectList(_context.Deceased, "Id", "FirstName");
                ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices, "Id", "Address");

                return Page();
            }

            // Καθαρισμός του ModelState για να αποφύγουμε προβλήματα με validation
            ModelState.Clear();

            try
            {
                _logger.LogInformation("Προσπάθεια ενημέρωσης Funeral");
                _context.Attach(Funeral).State = EntityState.Modified;
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
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η ενημέρωση της κηδείας.");

                    // Επαναφορά των dropdown λιστών
                    ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Address");
                    ViewData["DeceasedId"] = new SelectList(_context.Deceased, "Id", "FirstName");
                    ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices, "Id", "Address");

                    return Page();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Σφάλμα συγχρονισμού κατά την ενημέρωση της κηδείας");
                if (!FuneralExists(Funeral.Id))
                {
                    _logger.LogWarning($"Δεν βρέθηκε κηδεία με ID {Funeral.Id}");
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", $"Προέκυψε σφάλμα συγχρονισμού: {ex.Message}");

                    // Επαναφορά των dropdown λιστών
                    ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Address");
                    ViewData["DeceasedId"] = new SelectList(_context.Deceased, "Id", "FirstName");
                    ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices, "Id", "Address");

                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την ενημέρωση της κηδείας");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");

                // Επαναφορά των dropdown λιστών
                ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Address");
                ViewData["DeceasedId"] = new SelectList(_context.Deceased, "Id", "FirstName");
                ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices, "Id", "Address");

                return Page();
            }
        }

        private bool FuneralExists(int id)
        {
            return _context.Funerals.Any(e => e.Id == id);
        }
    }
}