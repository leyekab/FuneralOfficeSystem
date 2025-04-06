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
    public class IndexModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;

        public IndexModel(FuneralOfficeSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Funeral> Funeral { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Funeral = await _context.Funerals
                .Include(f => f.Client)
                .Include(f => f.Deceased)
                .Include(f => f.FuneralOffice).ToListAsync();
        }
    }
}
