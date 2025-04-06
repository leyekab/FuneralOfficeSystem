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
    public class DetailsModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;

        public DetailsModel(FuneralOfficeSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Deceased Deceased { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deceased = await _context.Deceased.FirstOrDefaultAsync(m => m.Id == id);
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
    }
}
