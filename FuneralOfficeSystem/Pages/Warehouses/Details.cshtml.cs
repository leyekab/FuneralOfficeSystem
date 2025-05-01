using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using FuneralOfficeSystem.ViewModels;

namespace FuneralOfficeSystem.Pages.Warehouses
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(ApplicationDbContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public WarehouseViewModel Warehouse { get; set; } = default!;

        // Για το dropdown προϊόντων στο modal
        public SelectList ProductsList { get; set; } = default!;

        // Για το dropdown γραφείων στη μεταφορά
        public SelectList FuneralOfficesList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var funeralOffice = await _context.FuneralOffices
                .FirstOrDefaultAsync(fo => fo.Id == Id);

            if (funeralOffice == null)
            {
                return NotFound();
            }

            // Δημιουργία του WarehouseViewModel
            Warehouse = new WarehouseViewModel
            {
                FuneralOfficeId = funeralOffice.Id,
                FuneralOfficeName = funeralOffice.Name
            };

            // Φόρτωση των προϊόντων της αποθήκης
            var inventoryItems = await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.FuneralOfficeId == Id)
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

            Warehouse.Items = inventoryItems;
            Warehouse.TotalValue = inventoryItems.Sum(i => i.TotalValue);
            Warehouse.TotalProducts = inventoryItems.Sum(i => i.Quantity);

            // Προετοιμασία των dropdowns
            ProductsList = new SelectList(
                await _context.Products
                    .Where(p => p.IsEnabled)
                    .OrderBy(p => p.Name)
                    .ToListAsync(),
                "Id",
                "Name"
            );

            FuneralOfficesList = new SelectList(
                await _context.FuneralOffices
                    .Where(fo => fo.IsEnabled && fo.Id != Id)
                    .OrderBy(fo => fo.Name)
                    .ToListAsync(),
                "Id",
                "Name"
            );

            return Page();
        }

        // Χειρισμός προσθήκης προϊόντος
        public async Task<IActionResult> OnPostAddStockAsync(int productId, int quantity, decimal unitPrice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.FuneralOfficeId == Id && i.ProductId == productId);

            if (inventory == null)
            {
                // Δημιουργία νέας εγγραφής στο inventory
                inventory = new Inventory
                {
                    FuneralOfficeId = Id,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    TotalValue = quantity * unitPrice
                };
                _context.Inventories.Add(inventory);
            }
            else
            {
                // Ενημέρωση υπάρχουσας εγγραφής
                inventory.Quantity += quantity;
                inventory.UnitPrice = unitPrice;
                inventory.TotalValue = inventory.Quantity * unitPrice;
            }

            // Καταγραφή της συναλλαγής
            var transaction = new InventoryTransaction
            {
                TransactionType = "Addition",
                TransactionTypeEnum = TransactionTypeEnum.Addition,
                Quantity = quantity,
                ProductId = productId,
                SourceFuneralOfficeId = Id,
                TransactionDate = DateTime.UtcNow
            };

            _context.InventoryTransactions.Add(transaction);

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        // Χειρισμός αφαίρεσης προϊόντος
        public async Task<IActionResult> OnPostRemoveStockAsync(int productId, int quantity)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.FuneralOfficeId == Id && i.ProductId == productId);

            if (inventory == null || inventory.Quantity < quantity)
            {
                return BadRequest("Ανεπαρκές απόθεμα");
            }

            inventory.Quantity -= quantity;
            inventory.TotalValue = inventory.Quantity * inventory.UnitPrice;

            // Καταγραφή της συναλλαγής
            var transaction = new InventoryTransaction
            {
                TransactionType = "Subtraction",
                TransactionTypeEnum = TransactionTypeEnum.Subtraction,
                Quantity = quantity,
                ProductId = productId,
                SourceFuneralOfficeId = Id,
                TransactionDate = DateTime.UtcNow
            };

            _context.InventoryTransactions.Add(transaction);

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        // Χειρισμός μεταφοράς προϊόντος
        public async Task<IActionResult> OnPostTransferStockAsync(int productId, int quantity, int destinationOfficeId)
        {
            var sourceInventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.FuneralOfficeId == Id && i.ProductId == productId);

            if (sourceInventory == null || sourceInventory.Quantity < quantity)
            {
                return BadRequest("Ανεπαρκές απόθεμα για μεταφορά");
            }

            var destinationInventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.FuneralOfficeId == destinationOfficeId && i.ProductId == productId);

            // Μείωση ποσότητας από την πηγή
            sourceInventory.Quantity -= quantity;
            sourceInventory.TotalValue = sourceInventory.Quantity * sourceInventory.UnitPrice;

            if (destinationInventory == null)
            {
                // Δημιουργία νέας εγγραφής στον προορισμό
                destinationInventory = new Inventory
                {
                    FuneralOfficeId = destinationOfficeId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = sourceInventory.UnitPrice,
                    TotalValue = quantity * sourceInventory.UnitPrice
                };
                _context.Inventories.Add(destinationInventory);
            }
            else
            {
                // Ενημέρωση υπάρχουσας εγγραφής στον προορισμό
                destinationInventory.Quantity += quantity;
                destinationInventory.TotalValue = destinationInventory.Quantity * destinationInventory.UnitPrice;
            }

            // Καταγραφή της συναλλαγής
            var transaction = new InventoryTransaction
            {
                TransactionType = "Transfer",
                TransactionTypeEnum = TransactionTypeEnum.Transfer,
                Quantity = quantity,
                ProductId = productId,
                SourceFuneralOfficeId = Id,
                DestinationFuneralOfficeId = destinationOfficeId,
                TransactionDate = DateTime.UtcNow
            };

            _context.InventoryTransactions.Add(transaction);

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}