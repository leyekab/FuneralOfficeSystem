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

namespace FuneralOfficeSystem.Pages.Churches
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
            Church = new Church
            {
                IsEnabled = true,
                Funerals = new List<Funeral>()
            };

            return Page();
        }

        [BindProperty]
        public Church Church { get; set; } = default!;

        [BindProperty]
        public bool IsPopup { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync εκτελείται");
            _logger.LogInformation($"Church Name: {Church?.Name}");
            _logger.LogInformation($"Church Address: {Church?.Address}");
            _logger.LogInformation($"Church Phone: {Church?.Phone}");
            _logger.LogInformation($"Church IsEnabled: {Church?.IsEnabled}");

            if (Church == null)
            {
                _logger.LogWarning("Church είναι null");
                ModelState.AddModelError("", "Η φόρμα δεν έστειλε δεδομένα");
                return Page();
            }

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

            Church.Funerals = new List<Funeral>();

            try
            {
                _logger.LogInformation("Προσπάθεια προσθήκης Church");
                _context.Churches.Add(Church);
                _logger.LogInformation("Προσπάθεια αποθήκευσης αλλαγών");
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation($"Αποτέλεσμα αποθήκευσης: {result} εγγραφές αποθηκεύτηκαν");

                if (result > 0)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return new JsonResult(new { success = true });
                    }
                    return IsPopup
                        ? Content("<script>window.close();</script>", "text/html")
                        : RedirectToPage("./Index");
                }
                else
                {
                    _logger.LogWarning("Δεν αποθηκεύτηκαν εγγραφές");
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η αποθήκευση της εκκλησίας.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την αποθήκευση της εκκλησίας");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Page();
            }
        }
    }
}