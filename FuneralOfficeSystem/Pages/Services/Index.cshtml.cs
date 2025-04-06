using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;

namespace FuneralOfficeSystem.Pages.Services
{
    public class IndexModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;

        public IndexModel(FuneralOfficeSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Service> Service { get; set; } = default!;

        public async Task OnGetAsync(string searchString)
        {
            IQueryable<Service> servicesQuery = _context.Services.Include(s => s.Supplier);

            if (!string.IsNullOrEmpty(searchString))
            {
                servicesQuery = servicesQuery.Where(s =>
                    s.Name.Contains(searchString) ||
                    (s.Description != null && s.Description.Contains(searchString)) ||
                    s.Category.Contains(searchString) ||
                    (s.Supplier != null && s.Supplier.Name.Contains(searchString)));
            }

            Service = await servicesQuery.ToListAsync();
        }
    }
}