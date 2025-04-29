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

        public IActionResult OnGet()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Funeral Funeral { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Funerals == null || Funeral == null)
            {
                ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
                ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices, "Id", "Name");
                return Page();
            }

            _context.Funerals.Add(Funeral);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}