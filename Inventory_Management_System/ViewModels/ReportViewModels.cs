using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory_Management_System.ViewModels
{
    public class StockHealthItemViewModel
    {
        public int ProductID { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int Threshold { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue => StockQuantity * UnitPrice;

        public string Status
        {
            get
            {
                if (StockQuantity == 0) return "Out of Stock";
                if (StockQuantity <= Threshold) return "Low Stock";
                return "In Stock";
            }
        }

        public bool IsRestockNeeded => StockQuantity <= Threshold;
    }

    public class StockHealthReportViewModel
    {
        [Display(Name = "Total Catalog Items")]
        public int TotalItemsCount { get; set; }

        [Display(Name = "Low Stock Count")]
        public int LowStockCount { get; set; }

        [Display(Name = "Out of Stock Count")]
        public int OutOfStockCount { get; set; }

        [Display(Name = "In Stock Count")]
        public int InStockCount => TotalItemsCount - LowStockCount - OutOfStockCount;

        [Display(Name = "Total Valuation Amount")]
        public decimal TotalValuationAmount { get; set; }

        public List<StockHealthItemViewModel> Items { get; set; } = new();

        public List<SelectListItem> Categories { get; set; } = new();

        // Filter criteria
        public int? CategoryID { get; set; }
        public string? StatusFilter { get; set; } = "All";
        public string? Search { get; set; }
    }

    public class SalesSummaryItemViewModel
    {
        public int SaleID { get; set; }
        public DateTime SaleDate { get; set; }
        public string CustomerInfo { get; set; } = "Walk-in Customer";
        public int ItemsCount { get; set; }
        public int TotalUnitsSold { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class SalesReportViewModel
    {
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string DatePreset { get; set; } = "ThisMonth";

        public string? Search { get; set; }

        [Display(Name = "Total Revenue")]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Total Orders")]
        public int TotalSalesCount { get; set; }

        [Display(Name = "Total Items Sold")]
        public int TotalUnitsSold { get; set; }

        [Display(Name = "Average Order Value")]
        public decimal AverageOrderValue { get; set; }

        public List<SalesSummaryItemViewModel> SalesRecords { get; set; } = new();
    }
}
