using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;

namespace FuneralOfficeSystem.Pages.Deceaseds
{
    public class DeleteModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;

        public DeleteModel(FuneralOfficeSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Deceased Deceased { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deceased = await _context.Deceaseds.FirstOrDefaultAsync(m => m.Id == id);

            if (deceased == null)
            {
                return NotFound();
            }
            else
            {
                Deceased = deceased;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deceased = await _context.Deceaseds.FindAsync(id);
            if (deceased != null)
            {
                Deceased = deceased;
                _context.Deceaseds.Remove(Deceased);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
