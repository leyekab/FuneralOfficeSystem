using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;

namespace FuneralOfficeSystem.Pages.Funerals
{
    public class CreateModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(FuneralOfficeSystem.Data.ApplicationDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            _logger.LogInformation("Προετοιμασία φόρμας δημιουργίας κηδείας");

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Address");
            ViewData["DeceasedId"] = new SelectList(_context.Deceased, "Id", "FirstName");
            ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices, "Id", "Address");

            return Page();
        }

        [BindProperty]
        public Funeral Funeral { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync εκτελείται");
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
                _logger.LogInformation("Προσπάθεια προσθήκης Funeral");
                _context.Funerals.Add(Funeral);
                _logger.LogInformation("Προσπάθεια αποθήκευσης αλλαγών");
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation($"Αποτέλεσμα αποθήκευσης: {result} εγγραφές αποθηκεύτηκαν");

                if (result > 0)
                {
                    _logger.LogInformation("Επιτυχής αποθήκευση, ανακατεύθυνση στο Index");
                    return RedirectToPage("./Index");
                }
                else
                {
                    _logger.LogWarning("Δεν αποθηκεύτηκαν εγγραφές");
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η αποθήκευση της κηδείας.");

                    // Επαναφορά των dropdown λιστών
                    ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Address");
                    ViewData["DeceasedId"] = new SelectList(_context.Deceased, "Id", "FirstName");
                    ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices, "Id", "Address");

                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την αποθήκευση της κηδείας");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");

                // Επαναφορά των dropdown λιστών
                ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Address");
                ViewData["DeceasedId"] = new SelectList(_context.Deceased, "Id", "FirstName");
                ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices, "Id", "Address");

                return Page();
            }
        }
    }
}