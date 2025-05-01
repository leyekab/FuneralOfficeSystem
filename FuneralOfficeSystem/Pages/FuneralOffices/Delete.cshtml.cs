using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;

namespace FuneralOfficeSystem.Pages.FuneralOffices
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
        public FuneralOffice FuneralOffice { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funeraloffice = await _context.FuneralOffices.FirstOrDefaultAsync(m => m.Id == id);

            if (funeraloffice == null)
            {
                return NotFound();
            }
            else
            {
                FuneralOffice = funeraloffice;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funeraloffice = await _context.FuneralOffices.FindAsync(id);
            if (funeraloffice != null)
            {
                _logger.LogInformation($"Απενεργοποίηση γραφείου τελετών: {funeraloffice.Name} (ID: {funeraloffice.Id})");

                // Αντί για διαγραφή, απενεργοποιούμε το γραφείο
                funeraloffice.IsEnabled = false;

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Επιτυχής απενεργοποίηση γραφείου τελετών: {funeraloffice.Name}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Σφάλμα κατά την απενεργοποίηση του γραφείου τελετών: {funeraloffice.Name}");
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
    }
}