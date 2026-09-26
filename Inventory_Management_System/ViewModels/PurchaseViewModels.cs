namespace Inventory_Management_System.ViewModels
{
    public class PurchaseListItemViewModel
    {
        public int PurchaseID { get; set; }

        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }

        public int SupplierID { get; set; }

        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; } = string.Empty;

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Items Count")]
        public int ItemsCount { get; set; }
    }

    public class PurchaseFilterViewModel
    {
        [Display(Name = "Search Supplier or ID")]
        public string? Search { get; set; }

        [Display(Name = "Supplier")]
        public int? SupplierID { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public List<SelectListItem> Suppliers { get; set; } = new();
    }

    public class PurchaseItemFormViewModel
    {
        public int PurchaseItemID { get; set; }

        [Required(ErrorMessage = "Please select a product")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid product")]
        [Display(Name = "Product")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10000, ErrorMessage = "Quantity must be between 1 and 10,000")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit cost is required")]
        [Range(0.01, 100000, ErrorMessage = "Unit cost must be greater than 0")]
        [Display(Name = "Unit Cost ($)")]
        public decimal UnitCost { get; set; }

        public decimal Subtotal => Quantity * UnitCost;
    }

    public class PurchaseFormViewModel
    {
        public int PurchaseID { get; set; }

        [Required(ErrorMessage = "Purchase Date is required")]
        [Display(Name = "Purchase Date")]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Please select a supplier")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid supplier")]
        [Display(Name = "Supplier")]
        public int SupplierID { get; set; }

        [Display(Name = "Total Amount ($)")]
        public decimal TotalAmount { get; set; }

        public List<PurchaseItemFormViewModel> Items { get; set; } = new();

        public List<SelectListItem> Suppliers { get; set; } = new();
        public List<SelectListItem> Products { get; set; } = new();
    }

    public class PurchaseItemDetailsViewModel
    {
        public int PurchaseItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Subtotal => Quantity * UnitCost;
    }

    public class PurchaseDetailsViewModel
    {
        public int PurchaseID { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        public List<PurchaseItemDetailsViewModel> Items { get; set; } = new();
    }

    public class PurchaseDeleteViewModel
    {
        public int PurchaseID { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }

        public List<PurchaseItemDetailsViewModel> Items { get; set; } = new();

        public bool CanDelete { get; set; } = true;
        public string BlockingReason { get; set; } = string.Empty;
    }
}