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

namespace FuneralOfficeSystem.Pages.Products
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
            // Initialize product with default values
            Product = new Product
            {
                IsEnabled = true,
                Inventories = new List<Inventory>(),
                FuneralProducts = new List<FuneralProduct>()
            };

            ViewData["SupplierId"] = new SelectList(_context.Suppliers
                .Where(s => s.SupplierType != SupplierType.Services && s.IsEnabled)
                .OrderBy(s => s.Name), "Id", "Name");

            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync εκτελείται");
            _logger.LogInformation($"Product Name: {Product?.Name}");
            _logger.LogInformation($"Product Category: {Product?.Category}");
            _logger.LogInformation($"Product SupplierId: {Product?.SupplierId}");
            _logger.LogInformation($"Product IsEnabled: {Product?.IsEnabled}");

            // Έλεγχος αν το μοντέλο είναι null
            if (Product == null)
            {
                _logger.LogWarning("Product είναι null");
                ModelState.AddModelError("", "Η φόρμα δεν έστειλε δεδομένα");
                ViewData["SupplierId"] = new SelectList(_context.Suppliers
                    .Where(s => s.SupplierType != SupplierType.Services && s.IsEnabled)
                    .OrderBy(s => s.Name), "Id", "Name");
                return Page();
            }

            // Έλεγχος για υποχρεωτικά πεδία
            if (string.IsNullOrWhiteSpace(Product.Name))
            {
                ModelState.AddModelError("Product.Name", "Το όνομα είναι υποχρεωτικό");
            }

            if (string.IsNullOrWhiteSpace(Product.Category))
            {
                ModelState.AddModelError("Product.Category", "Η κατηγορία είναι υποχρεωτική");
            }

            if (!ModelState.IsValid)
            {
                ViewData["SupplierId"] = new SelectList(_context.Suppliers
                    .Where(s => s.SupplierType != SupplierType.Services && s.IsEnabled)
                    .OrderBy(s => s.Name), "Id", "Name");
                return Page();
            }

            // Initialize navigation properties
            Product.Inventories = new List<Inventory>();
            Product.FuneralProducts = new List<FuneralProduct>();

            try
            {
                _logger.LogInformation("Προσπάθεια προσθήκης Product");
                _context.Products.Add(Product);
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
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η αποθήκευση του προϊόντος.");
                    ViewData["SupplierId"] = new SelectList(_context.Suppliers
                        .Where(s => s.SupplierType != SupplierType.Services && s.IsEnabled)
                        .OrderBy(s => s.Name), "Id", "Name");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την αποθήκευση του προϊόντος");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                ViewData["SupplierId"] = new SelectList(_context.Suppliers
                    .Where(s => s.SupplierType != SupplierType.Services && s.IsEnabled)
                    .OrderBy(s => s.Name), "Id", "Name");
                return Page();
            }
        }
    }
}