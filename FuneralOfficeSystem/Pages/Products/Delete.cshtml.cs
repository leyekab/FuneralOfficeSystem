using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;
using Microsoft.Extensions.Logging;

namespace FuneralOfficeSystem.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(FuneralOfficeSystem.Data.ApplicationDbContext context, ILogger<DeleteModel> logger)
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
                _logger.LogWarning("Προσπάθεια διαγραφής χωρίς καθορισμένο ID");
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια διαγραφής χωρίς καθορισμένο ID");
                return NotFound();
            }

            try
            {
                var product = await _context.Products
                    .Include(p => p.Inventories)
                    .Include(p => p.FuneralProducts)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (product != null)
                {
                    _logger.LogInformation($"Διαγραφή προϊόντος με ID {id}");

                    // Check if the product has related inventories or funeral products
                    if ((product.Inventories != null && product.Inventories.Any()) ||
                        (product.FuneralProducts != null && product.FuneralProducts.Any()))
                    {
                        _logger.LogWarning("Το προϊόν έχει συσχετισμένα αποθέματα ή κηδείες");

                        // Instead of deleting, set IsEnabled to false
                        product.IsEnabled = false;
                        _context.Attach(product).State = EntityState.Modified;
                        _logger.LogInformation("Απενεργοποίηση προϊόντος αντί για διαγραφή");
                    }
                    else
                    {
                        // No related entities, safe to delete
                        _context.Products.Remove(product);
                    }

                    await _context.SaveChangesAsync();
                }
                else
                {
                    _logger.LogWarning($"Δεν βρέθηκε προϊόν με ID {id} κατά τη διαγραφή");
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά τη διαγραφή προϊόντος");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Page();
            }
        }
    }
}