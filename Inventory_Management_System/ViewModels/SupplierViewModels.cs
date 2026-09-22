namespace Inventory_Management_System.ViewModels
{
    public class SupplierListItemViewModel
    {
        public int SupplierID { get; set; }

        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; } = string.Empty;

        [Display(Name = "Contact Name")]
        public string ContactName { get; set; } = string.Empty;

        [Display(Name = "Phone")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Supplied Products")]
        public int MappedProductsCount { get; set; }

        [Display(Name = "Purchases")]
        public int PurchasesCount { get; set; }
    }

    public class SupplierFilterViewModel
    {
        [Display(Name = "Search by Supplier or Contact")]
        public string? Search { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SupplierFormViewModel
    {
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Supplier Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Supplier Name must be between 2 and 100 characters")]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact Name is required")]
        [StringLength(100, ErrorMessage = "Contact Name cannot exceed 100 characters")]
        [Display(Name = "Contact Person")]
        public string ContactName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;
    }

    public class SupplierProductItemViewModel
    {
        public int SupplierProductID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SupplierSKU { get; set; } = string.Empty;
        public decimal ContractPrice { get; set; }
        public int LeadTimeDays { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
    }

    public class SupplierDetailsViewModel
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public int PurchasesCount { get; set; }
        public List<SupplierProductItemViewModel> MappedProducts { get; set; } = new();
    }

    public class SupplierDeleteViewModel
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public int PurchasesCount { get; set; }
        public int MappedProductsCount { get; set; }

        public bool CanDelete => PurchasesCount == 0;

        public string BlockingReason => CanDelete
            ? string.Empty
            : $"This supplier cannot be deleted because {PurchasesCount} purchase history record(s) exist for this supplier. Historical purchases must be preserved.";
    }
}
