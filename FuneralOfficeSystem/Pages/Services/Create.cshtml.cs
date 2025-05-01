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

namespace FuneralOfficeSystem.Pages.Services
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
            ViewData["SupplierId"] = new SelectList(_context.Suppliers.Where(s => s.SupplierType != SupplierType.Products), "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Service Service { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync εκτελείται");
            _logger.LogInformation($"Service Name: {Service?.Name ?? "NULL"}");
            _logger.LogInformation($"Service Category: {Service?.Category?.Name ?? "NULL"}");
            _logger.LogInformation($"Service Description: {Service?.Description ?? "NULL"}");
            _logger.LogInformation($"Service SupplierId: {Service?.SupplierId}");
            _logger.LogInformation($"Service IsEnabled: {Service?.IsEnabled}");

            if (!ModelState.IsValid || Service == null) // Προσθήκη ελέγχου null
            {
                _logger.LogWarning("Το ModelState δεν είναι έγκυρο ή το Service είναι null");
                ViewData["SupplierId"] = new SelectList(_context.Suppliers.Where(s => s.SupplierType != SupplierType.Products), "Id", "Name");
                return Page();
            }

            try
            {
                _logger.LogInformation("Προσθήκη νέας υπηρεσίας");
                _context.Services.Add(Service); // Τώρα είμαστε σίγουροι ότι το Service δεν είναι null
                await _context.SaveChangesAsync();
                _logger.LogInformation("Η υπηρεσία αποθηκεύτηκε με επιτυχία");

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την αποθήκευση της υπηρεσίας");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                ViewData["SupplierId"] = new SelectList(_context.Suppliers.Where(s => s.SupplierType != SupplierType.Products), "Id", "Name");
                return Page();
            }
        }
    }
}