using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory_Management_System.ViewModels
{
    public class SupplierProductListItemViewModel
    {
        public int SupplierProductID { get; set; }
        public int SupplierID { get; set; }

        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; } = string.Empty;

        public int ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "System SKU")]
        public string ProductSKU { get; set; } = string.Empty;

        [Display(Name = "Category")]
        public string CategoryName { get; set; } = string.Empty;

        [Display(Name = "Supplier SKU")]
        public string SupplierSKU { get; set; } = string.Empty;

        [Display(Name = "Contract Price")]
        public decimal ContractPrice { get; set; }

        [Display(Name = "Lead Time")]
        public int LeadTimeDays { get; set; }
    }

    public class SupplierProductFilterViewModel
    {
        [Display(Name = "Supplier")]
        public int? SupplierID { get; set; }

        [Display(Name = "Product")]
        public int? ProductID { get; set; }

        [Display(Name = "Search SKU / Keyword")]
        public string? Search { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public List<SelectListItem> Suppliers { get; set; } = new();
        public List<SelectListItem> Products { get; set; } = new();
    }

    public class SupplierProductFormViewModel
    {
        public int SupplierProductID { get; set; }

        [Required(ErrorMessage = "Supplier is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid supplier")]
        [Display(Name = "Supplier")]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Product is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid product")]
        [Display(Name = "Product")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Supplier SKU is required")]
        [StringLength(50, ErrorMessage = "Supplier SKU cannot exceed 50 characters")]
        [Display(Name = "Supplier SKU")]
        public string SupplierSKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contract Price is required")]
        [Range(0.01, 100000, ErrorMessage = "Contract Price must be greater than 0 and up to 100,000")]
        [Display(Name = "Contract Price ($)")]
        public decimal ContractPrice { get; set; }

        [Required(ErrorMessage = "Lead Time is required")]
        [Range(1, 365, ErrorMessage = "Lead Time must be between 1 and 365 days")]
        [Display(Name = "Lead Time (Days)")]
        public int LeadTimeDays { get; set; }

        public string? SupplierName { get; set; }
        public string? ProductName { get; set; }

        public List<SelectListItem> Suppliers { get; set; } = new();
        public List<SelectListItem> Products { get; set; } = new();
    }

    public class SupplierProductDeleteViewModel
    {
        public int SupplierProductID { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SupplierSKU { get; set; } = string.Empty;
        public decimal ContractPrice { get; set; }
        public int LeadTimeDays { get; set; }
    }
}
