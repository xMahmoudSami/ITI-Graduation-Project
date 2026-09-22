namespace Inventory_Management_System.ViewModels
{
    public class CategoryListItemViewModel
    {
        public int CategoryID { get; set; }

        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Products Count")]
        public int ProductsCount { get; set; }
    }

    public class CategoryFilterViewModel
    {
        [Display(Name = "Search Category")]
        public string? Search { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CategoryFormViewModel
    {
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category Name must be between 2 and 100 characters")]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }

    public class CategoryProductItemViewModel
    {
        public int ProductID { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public string StockStatus => StockQuantity == 0 ? "Out of Stock" : (StockQuantity <= LowStockThreshold ? "Low Stock" : "In Stock");
        public string StockBadgeClass => StockQuantity == 0 ? "badge-danger" : (StockQuantity <= LowStockThreshold ? "badge-warning" : "badge-success");
    }

    public class CategoryDetailsViewModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProductsCount => Products.Count;
        public List<CategoryProductItemViewModel> Products { get; set; } = new();
    }

    public class CategoryDeleteViewModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProductsCount { get; set; }
        public bool CanDelete => ProductsCount == 0;
        public string BlockingMessage => CanDelete
            ? string.Empty
            : $"This category contains {ProductsCount} product(s). Move or delete those products before deleting the category.";
    }
}
