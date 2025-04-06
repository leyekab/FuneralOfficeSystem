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

namespace FuneralOfficeSystem.Pages.Suppliers
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
        public Supplier Supplier { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια διαγραφής χωρίς καθορισμένο ID");
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .Include(s => s.Products)
                .Include(s => s.Services)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (supplier == null)
            {
                _logger.LogWarning($"Δεν βρέθηκε προμηθευτής με ID {id}");
                return NotFound();
            }

            Supplier = supplier;
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
                var supplier = await _context.Suppliers
                    .Include(s => s.Products)
                    .Include(s => s.Services)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (supplier != null)
                {
                    _logger.LogInformation($"Διαγραφή προμηθευτή με ID {id}");

                    // Check if the supplier has related products or services
                    if ((supplier.Products != null && supplier.Products.Any()) ||
                        (supplier.Services != null && supplier.Services.Any()))
                    {
                        _logger.LogWarning("Ο προμηθευτής έχει συσχετισμένα προϊόντα ή υπηρεσίες");

                        // Instead of deleting, set IsEnabled to false
                        supplier.IsEnabled = false;
                        _context.Attach(supplier).State = EntityState.Modified;
                        _logger.LogInformation("Απενεργοποίηση προμηθευτή αντί για διαγραφή");
                    }
                    else
                    {
                        // No related entities, safe to delete
                        _context.Suppliers.Remove(supplier);
                    }

                    await _context.SaveChangesAsync();
                }
                else
                {
                    _logger.LogWarning($"Δεν βρέθηκε προμηθευτής με ID {id} κατά τη διαγραφή");
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά τη διαγραφή προμηθευτή");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Page();
            }
        }
    }
}