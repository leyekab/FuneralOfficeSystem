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
    public class IndexModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;

        public IndexModel(FuneralOfficeSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<FuneralOffice> FuneralOffice { get; set; } = default!;

        public async Task OnGetAsync(string searchString)
        {
            IQueryable<FuneralOffice> funeralOfficesQuery = _context.FuneralOffices;

            if (!string.IsNullOrEmpty(searchString))
            {
                funeralOfficesQuery = funeralOfficesQuery.Where(s =>
                    (s.Name != null && s.Name.Contains(searchString)) ||
                    (s.Address != null && s.Address.Contains(searchString)) ||
                    (s.Phone != null && s.Phone.Contains(searchString)));
            }

            FuneralOffice = await funeralOfficesQuery.ToListAsync();
        }
    }
}