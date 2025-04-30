using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FuneralOfficeSystem.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public int FuneralOfficesCount { get; set; }
        public int SuppliersCount { get; set; }
        public int ProductsCount { get; set; }
        public int ServicesCount { get; set; }
        public int UsersCount { get; set; }
        public int ChurchesCount { get; set; }
        public int BurialPlacesCount { get; set; }
        public int FuneralsCount { get; set; }
        public int DeceasedsCount { get; set; }
        public int ClientsCount { get; set; }
        public int WarehousesCount { get; set; }
        public List<Funeral> RecentFunerals { get; set; } = new List<Funeral>();

        public async Task OnGetAsync()
        {
            // Φορτώνουμε τα μετρητικά στοιχεία για τα τετράγωνα
            FuneralOfficesCount = await _context.FuneralOffices.CountAsync();
            SuppliersCount = await _context.Suppliers.CountAsync();
            ProductsCount = await _context.Products.CountAsync();
            ServicesCount = await _context.Services.CountAsync();
            UsersCount = await _userManager.Users.CountAsync();
            ChurchesCount = await _context.Churches.CountAsync();
            BurialPlacesCount = await _context.BurialPlaces.CountAsync();
            FuneralsCount = await _context.Funerals.CountAsync();
            DeceasedsCount = await _context.Deceaseds.CountAsync();
            ClientsCount = await _context.Clients.CountAsync();
            WarehousesCount = await _context.Inventories.CountAsync();
            //ή αν θέλεις να δείξεις τον αριθμό των διαφορετικών αποθηκών(καθώς το Inventory είναι στην πραγματικότητα μια εγγραφή αποθήκης 
            //για ένα συγκεκριμένο προϊόν), θα μπορούσες να χρησιμοποιήσεις:
            //WarehousesCount = await _context.Inventories.Select(i => i.FuneralOfficeId).Distinct().CountAsync();
            //που θα μετρήσει τα μοναδικά γραφεία τελετών που έχουν εγγραφές αποθήκης.

            // Φορτώνουμε τις κηδείες των τελευταίων 90 ημερών
            var ninetyDaysAgo = DateTime.Today.AddDays(-90);
            RecentFunerals = await _context.Funerals
                .Include(f => f.Deceased)
                .Include(f => f.FuneralOffice)
                .Include(f => f.Client)
                .Include(f => f.BurialPlace)
                .Include(f => f.Church)
                .Where(f => f.FuneralDate >= ninetyDaysAgo)
                .OrderByDescending(f => f.FuneralDate)
                .ToListAsync();
        }
    }
}