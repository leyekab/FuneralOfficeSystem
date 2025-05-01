using System;
using System.Collections.Generic;
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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ApplicationDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Product Product { get; set; } = new();

        [BindProperty]
        public Dictionary<int, string> Properties { get; set; } = new();

        public SelectList Categories { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Initialize product with default values
            Product = new Product
            {
                IsEnabled = true,
                Properties = new List<ProductProperty>()
            };

            // Φόρτωση κατηγοριών
            LoadCategories();

            LoadSuppliers();

            return Page();
        }

        private void LoadCategories()
        {
            var categories = _context.ProductCategories
                .Where(c => c.IsEnabled)
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name })
                .ToList();

            Categories = new SelectList(categories, "Id", "Name");
        }

        private void LoadSuppliers()
        {
            ViewData["SupplierId"] = new SelectList(
                _context.Suppliers
                    .Where(s => s.SupplierType != SupplierType.Services && s.IsEnabled)
                    .OrderBy(s => s.Name)
                    .Select(s => new { s.Id, s.Name }),
                "Id",
                "Name"
            );
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadCategories();
                    LoadSuppliers();
                    return Page();
                }

                // Φόρτωση των ιδιοτήτων της κατηγορίας
                var categoryProperties = await _context.CategoryProperties
                    .Where(cp => cp.CategoryId == Product.CategoryId)
                    .ToListAsync();

                // Έλεγχος υποχρεωτικών ιδιοτήτων
                foreach (var prop in categoryProperties.Where(p => p.IsRequired))
                {
                    if (!Properties.ContainsKey(prop.Id) || string.IsNullOrWhiteSpace(Properties[prop.Id]))
                    {
                        ModelState.AddModelError($"Properties[{prop.Id}]", $"Η ιδιότητα {prop.Name} είναι υποχρεωτική");
                    }
                }

                if (!ModelState.IsValid)
                {
                    LoadCategories();
                    LoadSuppliers();
                    return Page();
                }

                // Δημιουργία των ProductProperty entities
                foreach (var prop in Properties.Where(p => !string.IsNullOrWhiteSpace(p.Value)))
                {
                    var categoryProp = categoryProperties.FirstOrDefault(cp => cp.Id == prop.Key);
                    if (categoryProp != null)
                    {
                        Product.Properties.Add(new ProductProperty
                        {
                            CategoryPropertyId = prop.Key,
                            Value = prop.Value
                        });
                    }
                }

                _context.Products.Add(Product);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά τη δημιουργία προϊόντος");
                ModelState.AddModelError("", "Προέκυψε σφάλμα κατά την αποθήκευση του προϊόντος");
                LoadCategories();
                LoadSuppliers();
                return Page();
            }
        }
    }
}