using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;

namespace FuneralOfficeSystem.Pages.Suppliers
{
    public class IndexModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;

        public IndexModel(FuneralOfficeSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Supplier> Supplier { get; set; } = default!;

        public async Task OnGetAsync(string searchString)
        {
            IQueryable<Supplier> suppliersQuery = _context.Suppliers;

            if (!string.IsNullOrEmpty(searchString))
            {
                suppliersQuery = suppliersQuery.Where(s =>
                    s.Name.Contains(searchString) ||
                    (s.Address != null && s.Address.Contains(searchString)) ||
                    s.Phone.Contains(searchString));
            }

            Supplier = await suppliersQuery.ToListAsync();
        }
    }
}