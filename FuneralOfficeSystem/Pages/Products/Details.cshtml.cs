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
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(ApplicationDbContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια προβολής προϊόντος χωρίς ID");
                return NotFound();
            }

            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Include(p => p.Properties)
                        .ThenInclude(pp => pp.CategoryProperty)
                    .Include(p => p.Inventories)
                    .Include(p => p.FuneralProducts)
                        .ThenInclude(fp => fp.Funeral)
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
    }
}