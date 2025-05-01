using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;
using FuneralOfficeSystem.ViewModels;

namespace FuneralOfficeSystem.Pages.Warehouses
{
    // Warehouses/Index.cshtml.cs
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<WarehouseViewModel> Warehouses { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Παίρνουμε όλα τα γραφεία τελετών
            var funeralOffices = await _context.FuneralOffices
                .Where(fo => fo.IsEnabled)
                .OrderBy(fo => fo.Name)
                .ToListAsync();

            foreach (var office in funeralOffices)
            {
                var warehouse = new WarehouseViewModel
                {
                    FuneralOfficeId = office.Id,
                    FuneralOfficeName = office.Name
                };

                // Παίρνουμε το inventory για το συγκεκριμένο γραφείο
                var inventoryItems = await _context.Inventories
                    .Include(i => i.Product)
                    .Where(i => i.FuneralOfficeId == office.Id)
                    .Select(i => new InventoryItemViewModel
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Category = i.Product.Category.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalValue = i.TotalValue,
                        Notes = i.Notes
                    })
                    .ToListAsync();

                warehouse.Items = inventoryItems;
                warehouse.TotalValue = inventoryItems.Sum(i => i.TotalValue);
                warehouse.TotalProducts = inventoryItems.Sum(i => i.Quantity);

                Warehouses.Add(warehouse);
            }
        }
    }
}
