using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{
    public class PurchaseViewModel
    {
        [Required(ErrorMessage = "Supplier is required")]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Purchase Date is required")]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; } = DateTime.Today;

        public List<PurchaseItemViewModel> Items { get; set; } = new List<PurchaseItemViewModel>();
    }

    public class PurchaseItemViewModel
    {
        [Required(ErrorMessage = "Product is required")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10000, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit Cost is required")]
        [Range(0.01, 100000, ErrorMessage = "Unit Cost must be greater than zero")]
        public decimal UnitCost { get; set; }
    }
}