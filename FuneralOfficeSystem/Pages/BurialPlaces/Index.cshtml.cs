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
namespace FuneralOfficeSystem.Pages.BurialPlaces
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
        public IList<BurialPlace> BurialPlace { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty; // Αρχικοποίηση με κενό string
        [BindProperty(SupportsGet = true)]
        public bool ShowDisabled { get; set; }
        public async Task OnGetAsync()
        {
            var burialPlacesQuery = _context.BurialPlaces.AsQueryable();
            // Apply search filter if search string is provided
            if (!string.IsNullOrEmpty(SearchString))
            {
                burialPlacesQuery = burialPlacesQuery.Where(c =>
                    c.Name.Contains(SearchString) ||
                    c.Address.Contains(SearchString) ||
                    (c.Phone != null && c.Phone.Contains(SearchString)));
            }
            // Filter disabled burial places unless ShowDisabled is true
            if (!ShowDisabled)
            {
                burialPlacesQuery = burialPlacesQuery.Where(c => c.IsEnabled);
            }
            BurialPlace = await burialPlacesQuery.ToListAsync();
        }
    }
}