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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ApplicationDbContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        [BindProperty]
        public Dictionary<int, string> Properties { get; set; } = new();

        public SelectList Categories { get; set; } = default!;

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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια επεξεργασίας χωρίς καθορισμένο ID");
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
                    _logger.LogWarning($"Δεν βρέθηκε προϊόν με ID {id}");
                    return NotFound();
                }

                Product = product;

                // Αρχικοποίηση του λεξικού Properties με τις υπάρχουσες τιμές
                foreach (var prop in Product.Properties)
                {
                    Properties[prop.CategoryPropertyId] = prop.Value;
                }

                LoadCategories();
                LoadSuppliers();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Σφάλμα κατά την φόρτωση του προϊόντος με ID {id}");
                return RedirectToPage("./Index");
            }
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

                var existingProduct = await _context.Products
                    .Include(p => p.Properties)
                    .FirstOrDefaultAsync(p => p.Id == Product.Id);

                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Ενημέρωση βασικών πεδίων
                existingProduct.Name = Product.Name;
                existingProduct.Description = Product.Description;
                existingProduct.CategoryId = Product.CategoryId;
                existingProduct.SupplierId = Product.SupplierId;
                existingProduct.IsEnabled = Product.IsEnabled;

                // Φόρτωση των ιδιοτήτων της κατηγορίας
                var categoryProperties = await _context.CategoryProperties
                    .Where(cp => cp.CategoryId == Product.CategoryId)
                    .ToListAsync();

                // Έλεγχος υποχρεωτικών ιδιοτήτων
                var hasValidationErrors = false;
                foreach (var prop in categoryProperties.Where(p => p.IsRequired))
                {
                    if (!Properties.ContainsKey(prop.Id) || string.IsNullOrWhiteSpace(Properties[prop.Id]))
                    {
                        ModelState.AddModelError($"Properties[{prop.Id}]",
                            $"Η ιδιότητα {prop.Name} είναι υποχρεωτική");
                        hasValidationErrors = true;
                    }
                }

                if (hasValidationErrors)
                {
                    LoadCategories();
                    LoadSuppliers();
                    return Page();
                }

                // Ενημέρωση ιδιοτήτων
                existingProduct.Properties.Clear();
                foreach (var prop in Properties.Where(p => !string.IsNullOrWhiteSpace(p.Value)))
                {
                    var categoryProp = categoryProperties.FirstOrDefault(cp => cp.Id == prop.Key);
                    if (categoryProp != null)
                    {
                        existingProduct.Properties.Add(new ProductProperty
                        {
                            CategoryPropertyId = prop.Key,
                            Value = prop.Value
                        });
                    }
                }

                // Καταγραφή της τελευταίας τροποποίησης
                existingProduct.LastModifiedAt = DateTime.UtcNow;
                existingProduct.LastModifiedBy = User.Identity?.Name ?? "System";

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Το προϊόν με ID {Product.Id} ενημερώθηκε επιτυχώς από τον χρήστη {User.Identity?.Name}");

                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Σφάλμα συγχρονισμού κατά την ενημέρωση του προϊόντος");
                ModelState.AddModelError("", "Το προϊόν έχει τροποποιηθεί από άλλο χρήστη. Παρακαλώ ανανεώστε τη σελίδα και προσπαθήστε ξανά.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την ενημέρωση προϊόντος");
                ModelState.AddModelError("", "Προέκυψε σφάλμα κατά την αποθήκευση του προϊόντος");
            }

            LoadCategories();
            LoadSuppliers();
            return Page();
        }
    }
}