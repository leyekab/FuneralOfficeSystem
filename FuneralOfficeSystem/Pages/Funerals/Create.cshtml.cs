using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;

namespace FuneralOfficeSystem.Pages.Funerals
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Funeral Funeral { get; set; } = default!;

        public void OnGet()
        {
            // Φορτώνουμε μόνο τη λίστα των Γραφείων Τελετών για το dropdown
            ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices.OrderBy(f => f.Name), "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Ξαναφορτώνουμε τη λίστα των Γραφείων Τελετών σε περίπτωση σφάλματος
                ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices.OrderBy(f => f.Name), "Id", "Name");
                return Page();
            }

            _context.Funerals.Add(Funeral);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}