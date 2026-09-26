namespace Inventory_Management_System.ViewModels
{
    public enum StockStatusFilter
    {
        All = 0,
        InStock = 1,
        LowStock = 2,
        OutOfStock = 3
    }

    public class ProductListItemViewModel
    {
        public int ProductID { get; set; }

        [Display(Name = "SKU")]
        public string SKU { get; set; } = string.Empty;

        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        public int CategoryID { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; } = string.Empty;

        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [Display(Name = "Threshold")]
        public int LowStockThreshold { get; set; }

        [Display(Name = "Suppliers")]
        public int MappedSuppliersCount { get; set; }

        [Display(Name = "Stock Status")]
        public string StockStatus => StockQuantity == 0 ? "Out of Stock" : (StockQuantity <= LowStockThreshold ? "Low Stock" : "In Stock");

        public string StockBadgeClass => StockQuantity == 0 ? "badge-danger" : (StockQuantity <= LowStockThreshold ? "badge-warning" : "badge-success");
    }

    public class ProductFilterViewModel
    {
        [Display(Name = "Search by Name or SKU")]
        public string? Search { get; set; }

        [Display(Name = "Category")]
        public int? CategoryID { get; set; }

        [Display(Name = "Stock Status")]
        public StockStatusFilter Status { get; set; } = StockStatusFilter.All;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public List<SelectListItem> Categories { get; set; } = new();
    }

    public class ProductFormViewModel
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
        [Display(Name = "SKU Code")]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(100, ErrorMessage = "Product Name cannot exceed 100 characters")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Category")]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Unit Price is required")]
        [Range(0.01, 100000, ErrorMessage = "Unit Price must be greater than 0 and up to 100,000")]
        [Display(Name = "Unit Price ($)")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Stock Quantity is required")]
        [Range(0, 10000, ErrorMessage = "Stock Quantity must be between 0 and 10,000")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Low Stock Threshold is required")]
        [Range(0, 1000, ErrorMessage = "Threshold must be between 0 and 1,000")]
        [Display(Name = "Low Stock Alert Threshold")]
        public int LowStockThreshold { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
    }

    public class ProductSupplierItemViewModel
    {
        public int SupplierProductID { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SupplierSKU { get; set; } = string.Empty;
        public decimal ContractPrice { get; set; }
        public int LeadTimeDays { get; set; }
    }

    public class ProductDetailsViewModel
    {
        public int ProductID { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }

        public string StockStatus => StockQuantity == 0 ? "Out of Stock" : (StockQuantity <= LowStockThreshold ? "Low Stock" : "In Stock");
        public string StockBadgeClass => StockQuantity == 0 ? "badge-danger" : (StockQuantity <= LowStockThreshold ? "badge-warning" : "badge-success");

        public int PurchaseItemsCount { get; set; }
        public int SaleItemsCount { get; set; }

        public List<ProductSupplierItemViewModel> Suppliers { get; set; } = new();
    }

    public class ProductDeleteViewModel
    {
        public int ProductID { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }

        public int PurchaseItemsCount { get; set; }
        public int SaleItemsCount { get; set; }
        public int SupplierProductsCount { get; set; }

        public bool CanDelete => PurchaseItemsCount == 0 && SaleItemsCount == 0;

        public string BlockingReason
        {
            get
            {
                if (CanDelete) return string.Empty;
                var reasons = new List<string>();
                if (PurchaseItemsCount > 0) reasons.Add($"{PurchaseItemsCount} purchase history record(s)");
                if (SaleItemsCount > 0) reasons.Add($"{SaleItemsCount} sales history record(s)");
                return $"This product cannot be deleted because it has {string.Join(" and ", reasons)}. Historical transactions must be preserved.";
            }
        }
    }
}
