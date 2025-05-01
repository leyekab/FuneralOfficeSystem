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

namespace FuneralOfficeSystem.Pages.Services
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
        public Service Service { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (service == null)
            {
                return NotFound();
            }
            Service = service;
            ViewData["SupplierId"] = new SelectList(_context.Suppliers.Where(s => s.SupplierType != SupplierType.Products), "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync της Edit εκτελείται");
            _logger.LogInformation($"Service ID: {Service?.Id}");
            _logger.LogInformation($"Service Name: {Service?.Name ?? "NULL"}");
            _logger.LogInformation($"Service Category: {Service?.Category?.Name ?? "NULL"}");
            _logger.LogInformation($"Service Description: {Service?.Description ?? "NULL"}");
            _logger.LogInformation($"Service SupplierId: {Service?.SupplierId}");
            _logger.LogInformation($"Service IsEnabled: {Service?.IsEnabled}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Το ModelState δεν είναι έγκυρο");
                ViewData["SupplierId"] = new SelectList(_context.Suppliers.Where(s => s.SupplierType != SupplierType.Products), "Id", "Name");
                return Page();
            }

            if (Service == null)
            {
                _logger.LogError("Το Service είναι null");
                return NotFound();
            }

            try
            {
                _logger.LogInformation("Ενημέρωση υπηρεσίας");
                var serviceToUpdate = await _context.Services.FindAsync(Service.Id);

                if (serviceToUpdate == null)
                {
                    return NotFound();
                }

                _context.Entry(serviceToUpdate).CurrentValues.SetValues(Service);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Η υπηρεσία ενημερώθηκε με επιτυχία");

                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Σφάλμα συγχρονισμού κατά την ενημέρωση της υπηρεσίας");

                if (!ServiceExists(Service.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", "Η υπηρεσία τροποποιήθηκε από άλλο χρήστη. Προσπαθήστε ξανά.");
                    ViewData["SupplierId"] = new SelectList(_context.Suppliers.Where(s => s.SupplierType != SupplierType.Products), "Id", "Name");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την ενημέρωση της υπηρεσίας");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                ViewData["SupplierId"] = new SelectList(_context.Suppliers.Where(s => s.SupplierType != SupplierType.Products), "Id", "Name");
                return Page();
            }
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}