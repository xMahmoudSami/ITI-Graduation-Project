using InventoryManagementSystem.Models;

namespace Inventory_Management_System.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
        [Unique]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(100, ErrorMessage = "Product Name cannot exceed 100 characters")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Unit Price is required")]
        [Range(0.01, 100000, ErrorMessage = "Price must be greater than zero")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Stock Quantity is required")]
        [Range(0, 10000, ErrorMessage = "Stock Quantity must be between 0 and 10000")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Low Stock Threshold is required")]
        [Range(0, 1000, ErrorMessage = "Threshold must be between 0 and 1000")]
        public int LowStockThreshold { get; set; }


        public Category? Category { get; set; }
        public ICollection<SupplierProduct> SupplierProducts { get; set; }
        public ICollection<PurchaseItem> PurchaseItems { get; set; }
        public ICollection<SaleItem> SaleItems { get; set; }
    }
}