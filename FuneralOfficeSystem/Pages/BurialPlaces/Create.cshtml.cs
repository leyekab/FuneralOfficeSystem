using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;
using Microsoft.Extensions.Logging;

namespace FuneralOfficeSystem.Pages.BurialPlaces
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
            // Initialize burial place with default values
            BurialPlace = new BurialPlace
            {
                IsEnabled = true,
                Funerals = new List<Funeral>()
            };

            return Page();
        }

        [BindProperty]
        public BurialPlace BurialPlace { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync εκτελείται");
            _logger.LogInformation($"BurialPlace Name: {BurialPlace?.Name}");
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

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Initialize navigation properties
            BurialPlace.Funerals = new List<Funeral>();

            try
            {
                _logger.LogInformation("Προσπάθεια προσθήκης BurialPlace");
                _context.BurialPlaces.Add(BurialPlace);
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
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η αποθήκευση του κοιμητηρίου.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την αποθήκευση του κοιμητηρίου");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Page();
            }
        }
    }
}