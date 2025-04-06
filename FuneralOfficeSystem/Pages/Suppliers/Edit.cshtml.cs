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

namespace FuneralOfficeSystem.Pages.Suppliers
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
        public Supplier Supplier { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .Include(s => s.Products)
                .Include(s => s.Services)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (supplier == null)
            {
                return NotFound();
            }

            Supplier = supplier;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Μέθοδος OnPostAsync της Edit εκτελείται");
            _logger.LogInformation($"Supplier ID: {Supplier?.Id}");
            _logger.LogInformation($"Supplier Name: {Supplier?.Name ?? "NULL"}");
            _logger.LogInformation($"Supplier Address: {Supplier?.Address ?? "NULL"}");
            _logger.LogInformation($"Supplier Phone: {Supplier?.Phone ?? "NULL"}");
            _logger.LogInformation($"SupplierType: {Supplier?.SupplierType}");
            _logger.LogInformation($"IsEnabled: {Supplier?.IsEnabled}");

            // Έλεγχος αν το μοντέλο είναι null
            if (Supplier == null)
            {
                _logger.LogWarning("Supplier είναι null");
                ModelState.AddModelError("", "Η φόρμα δεν έστειλε δεδομένα");
                return Page();
            }

            // Καθαρισμός του ModelState για να αποφύγουμε προβλήματα με validation
            ModelState.Clear();

            try
            {
                // Get the existing supplier to preserve navigation properties
                var existingSupplier = await _context.Suppliers
                    .Include(s => s.Products)
                    .Include(s => s.Services)
                    .FirstOrDefaultAsync(m => m.Id == Supplier.Id);

                if (existingSupplier == null)
                {
                    return NotFound();
                }

                // Update properties
                existingSupplier.Name = Supplier.Name;
                existingSupplier.Address = Supplier.Address;
                existingSupplier.Phone = Supplier.Phone;
                existingSupplier.SupplierType = Supplier.SupplierType;
                existingSupplier.IsEnabled = Supplier.IsEnabled;

                _logger.LogInformation("Προσπάθεια αποθήκευσης αλλαγών");
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation($"Αποτέλεσμα αποθήκευσης: {result} εγγραφές αποθηκεύτηκαν");

                if (result > 0)
                {
                    _logger.LogInformation("Επιτυχής ενημέρωση, ανακατεύθυνση στο Index");
                    return RedirectToPage("./Index");
                }
                else
                {
                    _logger.LogWarning("Δεν αποθηκεύτηκαν αλλαγές");
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η ενημέρωση του προμηθευτή.");
                    return Page();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Σφάλμα συγχρονισμού κατά την ενημέρωση του προμηθευτή");
                if (!SupplierExists(Supplier.Id))
                {
                    _logger.LogWarning($"Δεν βρέθηκε προμηθευτής με ID {Supplier.Id}");
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", $"Προέκυψε σφάλμα συγχρονισμού: {ex.Message}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Σφάλμα κατά την ενημέρωση του προμηθευτή");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Page();
            }
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.Id == id);
        }
    }
}