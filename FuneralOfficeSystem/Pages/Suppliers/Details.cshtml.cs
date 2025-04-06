using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;
using Microsoft.Extensions.Logging;

namespace FuneralOfficeSystem.Pages.Suppliers
{
    public class DetailsModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(FuneralOfficeSystem.Data.ApplicationDbContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Supplier Supplier { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Προσπάθεια προβολής λεπτομερειών χωρίς καθορισμένο ID");
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .Include(s => s.Products)
                .Include(s => s.Services)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (supplier == null)
            {
                _logger.LogWarning($"Δεν βρέθηκε προμηθευτής με ID {id}");
                return NotFound();
            }

            Supplier = supplier;
            return Page();
        }
    }
}