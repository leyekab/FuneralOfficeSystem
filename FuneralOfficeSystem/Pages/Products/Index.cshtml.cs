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
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly int PageSize = 10;

        public IndexModel(ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<Product> Products { get; set; } = new List<Product>();
        public SelectList Categories { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public bool ShowDisabled { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedCategoryId { get; set; }

        public string NameSort { get; set; } = string.Empty;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }

        public async Task OnGetAsync(string sortOrder, int? pageIndex)
        {
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            CurrentPage = pageIndex ?? 1;

            try
            {
                // Φόρτωση κατηγοριών για το dropdown
                var categoriesQuery = _context.ProductCategories
                    .Where(c => c.IsEnabled)
                    .OrderBy(c => c.Name)
                    .Select(c => new { c.Id, c.Name });

                Categories = new SelectList(await categoriesQuery.ToListAsync(), "Id", "Name");

                // Βασικό query για προϊόντα
                var query = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Include(p => p.Properties)
                        .ThenInclude(pp => pp.CategoryProperty)
                    .AsNoTracking() // Για καλύτερη απόδοση σε read-only λειτουργίες
                    .AsQueryable();

                // Φιλτράρισμα με βάση την κατάσταση ενεργοποίησης
                if (!ShowDisabled)
                {
                    query = query.Where(p => p.IsEnabled);
                }

                // Φιλτράρισμα με βάση την κατηγορία
                if (SelectedCategoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == SelectedCategoryId.Value);
                }

                // Φιλτράρισμα με βάση την αναζήτηση
                if (!string.IsNullOrEmpty(SearchString))
                {
                    var searchTerm = SearchString.ToLower();
                    query = query.Where(p =>
                        p.Name.ToLower().Contains(searchTerm) ||
                        (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                        (p.Category != null && p.Category.Name.ToLower().Contains(searchTerm)) ||
                        (p.Supplier != null && p.Supplier.Name.ToLower().Contains(searchTerm)) ||
                        p.Properties.Any(pp => pp.Value.ToLower().Contains(searchTerm))
                    );
                }

                // Ταξινόμηση
                query = sortOrder switch
                {
                    "name_desc" => query.OrderByDescending(p => p.Name),
                    _ => query.OrderBy(p => p.Name)
                };

                // Υπολογισμός σελίδων
                var totalItems = await query.CountAsync();
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

                // Εφαρμογή paging
                Products = await query
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();

                _logger.LogInformation($"Ανακτήθηκαν {Products.Count} προϊόντα για τη σελίδα {CurrentPage}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την ανάκτηση προϊόντων");
                ModelState.AddModelError("", "Προέκυψε σφάλμα κατά την ανάκτηση των προϊόντων.");
                Products = new List<Product>();
            }
        }
    }
}