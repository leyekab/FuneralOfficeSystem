using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;

namespace FuneralOfficeSystem.Pages.Funerals
{
    public class DeleteModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;

        public DeleteModel(FuneralOfficeSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Funeral Funeral { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funeral = await _context.Funerals.FirstOrDefaultAsync(m => m.Id == id);

            if (funeral == null)
            {
                return NotFound();
            }
            else
            {
                Funeral = funeral;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funeral = await _context.Funerals.FindAsync(id);
            if (funeral != null)
            {
                Funeral = funeral;
                _context.Funerals.Remove(Funeral);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
