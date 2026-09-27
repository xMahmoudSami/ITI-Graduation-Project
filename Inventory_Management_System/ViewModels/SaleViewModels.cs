using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory_Management_System.ViewModels
{
    public class SaleListItemViewModel
    {
        public int SaleID { get; set; }

        [Display(Name = "Sale Date")]
        public DateTime SaleDate { get; set; }

        [Display(Name = "Customer Info")]
        public string CustomerInfo { get; set; } = "Walk-in Customer";

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Items Count")]
        public int ItemsCount { get; set; }

        [Display(Name = "Total Units Sold")]
        public int TotalUnits { get; set; }
    }

    public class SaleFilterViewModel
    {
        [Display(Name = "Search Customer or ID")]
        public string? Search { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SaleItemFormViewModel
    {
        public int SaleItemID { get; set; }

        [Required(ErrorMessage = "Please select a product")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid product")]
        [Display(Name = "Product")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10000, ErrorMessage = "Quantity must be between 1 and 10,000")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, 100000, ErrorMessage = "Unit price must be greater than 0")]
        [Display(Name = "Unit Price ($)")]
        public decimal UnitPrice { get; set; }

        public decimal Subtotal => Quantity * UnitPrice;

        // Helper metadata for client-side display & validation
        public string? ProductName { get; set; }
        public int AvailableStock { get; set; }
    }

    public class SaleProductOptionViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
    }

    public class SaleFormViewModel
    {
        public int SaleID { get; set; }

        [Required(ErrorMessage = "Sale Date is required")]
        [Display(Name = "Sale Date")]
        [DataType(DataType.Date)]
        public DateTime SaleDate { get; set; } = DateTime.Today;

        [StringLength(200, ErrorMessage = "Customer info cannot exceed 200 characters")]
        [Display(Name = "Customer Info")]
        public string? CustomerInfo { get; set; }

        [Display(Name = "Total Amount ($)")]
        public decimal TotalAmount { get; set; }

        public List<SaleItemFormViewModel> Items { get; set; } = new();

        public List<SelectListItem> Products { get; set; } = new();

        public List<SaleProductOptionViewModel> ProductCatalog { get; set; } = new();
    }

    public class SaleItemDetailsViewModel
    {
        public int SaleItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }

    public class SaleDetailsViewModel
    {
        public int SaleID { get; set; }
        public DateTime SaleDate { get; set; }
        public string CustomerInfo { get; set; } = "Walk-in Customer";
        public decimal TotalAmount { get; set; }
        public List<SaleItemDetailsViewModel> Items { get; set; } = new();

        public int TotalQuantity => Items.Sum(i => i.Quantity);

        // Store details for print invoice layout
        public string InvoiceNumber => $"INV-{SaleID:D6}";
        public string StoreName { get; set; } = "IMS Enterprise";
        public string StoreAddress { get; set; } = "100 Logistics Way, Suite 400";
        public string StorePhone { get; set; } = "+1 (800) 555-0199";
        public string StoreEmail { get; set; } = "sales@ims-system.local";
        public string PaymentStatus { get; set; } = "Paid";
    }

    public class SaleDeleteViewModel
    {
        public int SaleID { get; set; }
        public DateTime SaleDate { get; set; }
        public string CustomerInfo { get; set; } = "Walk-in Customer";
        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }
        public List<SaleItemDetailsViewModel> Items { get; set; } = new();

        public bool CanDelete { get; set; } = true;
        public string BlockingReason { get; set; } = string.Empty;
    }
}
