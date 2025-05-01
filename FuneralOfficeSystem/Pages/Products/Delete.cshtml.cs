using System;
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
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(ApplicationDbContext context, ILogger<DeleteModel> logger)
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
                _logger.LogWarning("Προσπάθεια απενεργοποίησης προϊόντος χωρίς ID");
                return NotFound();
            }

            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Include(p => p.Properties)
                        .ThenInclude(pp => pp.CategoryProperty)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (product == null)
                {
                    _logger.LogWarning($"Προϊόν με ID {id} δε βρέθηκε");
                    return NotFound();
                }

                Product = product;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Σφάλμα κατά την ανάκτηση του προϊόντος με ID {id}");
                TempData["ErrorMessage"] = "Προέκυψε σφάλμα κατά την ανάκτηση του προϊόντος.";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                // Απενεργοποίηση του προϊόντος αντί για διαγραφή
                product.IsEnabled = false;
                product.LastModifiedAt = DateTime.UtcNow;
                product.LastModifiedBy = User.Identity?.Name ?? "System";

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Απενεργοποίηση προϊόντος: ID={product.Id}, Name={product.Name}, User={User.Identity?.Name}, Time={DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                TempData["SuccessMessage"] = $"Το προϊόν '{product.Name}' απενεργοποιήθηκε επιτυχώς.";

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Σφάλμα κατά την απενεργοποίηση του προϊόντος με ID {id}");
                TempData["ErrorMessage"] = "Προέκυψε σφάλμα κατά την απενεργοποίηση του προϊόντος.";
                return RedirectToPage("./Index");
            }
        }
    }
}