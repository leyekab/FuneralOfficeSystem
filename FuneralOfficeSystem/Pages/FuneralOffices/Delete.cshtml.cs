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
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;

        public DeleteModel(FuneralOfficeSystem.Data.ApplicationDbContext context)
        {
            _context = context;
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
                FuneralOffice = funeraloffice;
                _context.FuneralOffices.Remove(FuneralOffice);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}