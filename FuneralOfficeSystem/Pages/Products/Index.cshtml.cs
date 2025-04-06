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
    public class IndexModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(FuneralOfficeSystem.Data.ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<Product> Product { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public bool ShowDisabled { get; set; } = false;

        public async Task OnGetAsync(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                SearchString = searchString;
            }

            _logger.LogInformation($"Αναζήτηση προϊόντων με φίλτρο: {SearchString}");
            _logger.LogInformation($"Εμφάνιση απενεργοποιημένων: {ShowDisabled}");

            IQueryable<Product> productsQuery = _context.Products
                .Include(p => p.Supplier);

            // Φιλτράρισμε με βάση την κατάσταση ενεργοποίησης
            if (!ShowDisabled)
            {
                productsQuery = productsQuery.Where(p => p.IsEnabled);
            }

            // Φιλτράρισμα με βάση την αναζήτηση
            if (!string.IsNullOrEmpty(SearchString))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(SearchString) ||
                    (p.Description != null && p.Description.Contains(SearchString)) ||
                    p.Category.Contains(SearchString) ||
                    (p.Supplier != null && p.Supplier.Name.Contains(SearchString)));
            }

            // Ταξινόμηση με βάση την κατάσταση και το όνομα
            productsQuery = productsQuery
                .OrderByDescending(p => p.IsEnabled)
                .ThenBy(p => p.Name);

            Product = await productsQuery.ToListAsync();

            _logger.LogInformation($"Βρέθηκαν {Product.Count} προϊόντα");
        }
    }
}