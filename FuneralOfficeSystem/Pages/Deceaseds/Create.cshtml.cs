﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;

namespace FuneralOfficeSystem.Pages.Deceaseds
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
            return Page();
        }

        [BindProperty]
        public Deceased Deceased { get; set; } = default!;

        [BindProperty]
        public bool IsPopup { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync εκτελείται");
            _logger.LogInformation($"Deceased FirstName: {Deceased?.FirstName ?? "NULL"}");
            _logger.LogInformation($"Deceased LastName: {Deceased?.LastName ?? "NULL"}");
            _logger.LogInformation($"Deceased DeathDate: {Deceased?.DeathDate.ToString() ?? "NULL"}");

            // Έλεγχος αν το μοντέλο είναι null
            if (Deceased == null)
            {
                _logger.LogWarning("Deceased είναι null");
                ModelState.AddModelError("", "Η φόρμα δεν έστειλε δεδομένα");
                return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                    ? new JsonResult(new { success = false })
                    : Page() as IActionResult;
            }

            // Καθαρισμός του ModelState για να αποφύγουμε προβλήματα με validation
            ModelState.Clear();

            try
            {
                _logger.LogInformation("Προσπάθεια προσθήκης Deceased");
                _context.Deceaseds.Add(Deceased);
                _logger.LogInformation("Προσπάθεια αποθήκευσης αλλαγών");
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation($"Αποτέλεσμα αποθήκευσης: {result} εγγραφές αποθηκεύτηκαν");

                if (result > 0)
                {
                    _logger.LogInformation("Επιτυχής αποθήκευση");
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
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η αποθήκευση του αποβιώσαντα.");
                    return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                        ? new JsonResult(new { success = false })
                        : Page() as IActionResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την αποθήκευση του αποβιώσαντα");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                    ? new JsonResult(new { success = false })
                    : Page() as IActionResult;
            }
        }
    }
}