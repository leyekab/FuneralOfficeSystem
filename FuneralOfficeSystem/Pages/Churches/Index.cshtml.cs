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

namespace FuneralOfficeSystem.Pages.Churches
{
    public class IndexModel : PageModel
    {
        private readonly FuneralOfficeSystem.Data.ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(FuneralOfficeSystem.Data.ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<Church> Church { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty; // Αρχικοποίηση με κενό string

        [BindProperty(SupportsGet = true)]
        public bool ShowDisabled { get; set; }

        public async Task OnGetAsync()
        {
            var churchesQuery = _context.Churches.AsQueryable();

            // Apply search filter if search string is provided
            if (!string.IsNullOrEmpty(SearchString))
            {
                churchesQuery = churchesQuery.Where(c =>
                    c.Name.Contains(SearchString) ||
                    c.Address.Contains(SearchString) ||
                    (c.Phone != null && c.Phone.Contains(SearchString)));
            }

            // Filter disabled churches unless ShowDisabled is true
            if (!ShowDisabled)
            {
                churchesQuery = churchesQuery.Where(c => c.IsEnabled);
            }

            Church = await churchesQuery.ToListAsync();
        }
    }
}