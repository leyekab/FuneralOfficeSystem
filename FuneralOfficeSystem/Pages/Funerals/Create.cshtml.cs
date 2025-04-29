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
            // Αρχικοποίηση των SelectList για τα dropdowns
            ViewData["ClientId"] = new SelectList(_context.Clients.OrderBy(c => c.LastName), "Id", "FullName");
            ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices.OrderBy(f => f.Name), "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Επαναφόρτωση των SelectList σε περίπτωση σφάλματος
                ViewData["ClientId"] = new SelectList(_context.Clients.OrderBy(c => c.LastName), "Id", "FullName");
                ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices.OrderBy(f => f.Name), "Id", "Name");
                return Page();
            }

            _context.Funerals.Add(Funeral);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}