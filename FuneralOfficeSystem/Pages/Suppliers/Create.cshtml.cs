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

namespace FuneralOfficeSystem.Pages.Suppliers
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
            // Initialize navigation properties to avoid null reference exceptions
            Supplier = new Supplier
            {
                Products = new List<Product>(),
                Services = new List<Service>(),
                IsEnabled = true
            };

            return Page();
        }

        [BindProperty]
        public Supplier Supplier { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync εκτελείται");
            _logger.LogInformation($"Supplier Name: {Supplier?.Name ?? "NULL"}");
            _logger.LogInformation($"Supplier Address: {Supplier?.Address ?? "NULL"}");
            _logger.LogInformation($"Supplier Phone: {Supplier?.Phone ?? "NULL"}");
            _logger.LogInformation($"SupplierType: {Supplier?.SupplierType}");
            _logger.LogInformation($"IsEnabled: {Supplier?.IsEnabled}");

            // Έλεγχος αν το μοντέλο είναι null
            if (Supplier == null)
            {
                _logger.LogWarning("Supplier είναι null");
                ModelState.AddModelError("", "Η φόρμα δεν έστειλε δεδομένα");
                return Page();
            }

            // Initialize navigation properties to avoid null reference exceptions
            Supplier.Products = new List<Product>();
            Supplier.Services = new List<Service>();

            // Καθαρισμός του ModelState για να αποφύγουμε προβλήματα με validation
            ModelState.Clear();

            try
            {
                _logger.LogInformation("Προσπάθεια προσθήκης Supplier");
                _context.Suppliers.Add(Supplier);
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
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η αποθήκευση του προμηθευτή.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την αποθήκευση του προμηθευτή");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Page();
            }
        }
    }
}