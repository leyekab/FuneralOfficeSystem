namespace FuneralOfficeSystem.ViewModels
{
    public class WarehouseViewModel
    {
        public int FuneralOfficeId { get; set; }
        public string FuneralOfficeName { get; set; } = string.Empty;
        public List<InventoryItemViewModel> Items { get; set; } = new List<InventoryItemViewModel>();
        public decimal TotalValue { get; set; }
        public int TotalProducts { get; set; }
    }
}
