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

namespace FuneralOfficeSystem.Pages.Products
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
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια επεξεργασίας χωρίς καθορισμένο ID");
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Supplier)
                .Include(p => p.Inventories)
                .Include(p => p.FuneralProducts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                _logger.LogWarning($"Δεν βρέθηκε προϊόν με ID {id}");
                return NotFound();
            }

            Product = product;
            ViewData["SupplierId"] = new SelectList(_context.Suppliers
                .Where(s => s.SupplierType != SupplierType.Services && s.IsEnabled)
                .OrderBy(s => s.Name), "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync της Edit εκτελείται");
            _logger.LogInformation($"Product ID: {Product?.Id}");
            _logger.LogInformation($"Product Name: {Product?.Name}");
            _logger.LogInformation($"Product Category: {Product?.Category}");
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

            try
            {
                // Get the existing product to preserve navigation properties
                var existingProduct = await _context.Products
                    .Include(p => p.Inventories)
                    .Include(p => p.FuneralProducts)
                    .FirstOrDefaultAsync(m => m.Id == Product.Id);

                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Update properties
                existingProduct.Name = Product.Name;
                existingProduct.Description = Product.Description;
                existingProduct.Category = Product.Category;
                existingProduct.SupplierId = Product.SupplierId;
                existingProduct.IsEnabled = Product.IsEnabled;

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
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η ενημέρωση του προϊόντος.");
                    ViewData["SupplierId"] = new SelectList(_context.Suppliers
                        .Where(s => s.SupplierType != SupplierType.Services && s.IsEnabled)
                        .OrderBy(s => s.Name), "Id", "Name");
                    return Page();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Σφάλμα συγχρονισμού κατά την ενημέρωση του προϊόντος");
                if (!ProductExists(Product.Id))
                {
                    _logger.LogWarning($"Δεν βρέθηκε προϊόν με ID {Product.Id}");
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", $"Προέκυψε σφάλμα συγχρονισμού: {ex.Message}");
                    ViewData["SupplierId"] = new SelectList(_context.Suppliers
                        .Where(s => s.SupplierType != SupplierType.Services && s.IsEnabled)
                        .OrderBy(s => s.Name), "Id", "Name");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την ενημέρωση του προϊόντος");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                ViewData["SupplierId"] = new SelectList(_context.Suppliers
                    .Where(s => s.SupplierType != SupplierType.Services && s.IsEnabled)
                    .OrderBy(s => s.Name), "Id", "Name");
                return Page();
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}