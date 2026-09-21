namespace Inventory_Management_System.ViewModels
{
    public class DashboardViewModels
    {
        // 1. KPI Cards Metrics
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalStockQuantity { get; set; }
        public int LowStockCount { get; set; }

        public int TotalPurchasesCount { get; set; }
        public decimal TotalPurchasesValue { get; set; }

        public int TotalSalesCount { get; set; }
        public decimal TotalSalesRevenue { get; set; }

        // 2. Data Lists
        public List<LowStockProductViewModel> LowStockProducts { get; set; } = new();
        public List<TopSellingProductViewModel> TopSellingProducts { get; set; } = new();
        public List<RecentActivityViewModel> RecentActivities { get; set; } = new();
    }

    public class LowStockProductViewModel
    {
        public int ProductID { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
    }

    public class TopSellingProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RecentActivityViewModel
    {
        public string ActivityType { get; set; } = string.Empty;
        public int ReferenceID { get; set; }
        public DateTime ActivityDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string EntityInfo { get; set; } = string.Empty;
    }

    // View Models for Chart.js API Responses
    public class SalesVsPurchasesChartViewModel
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> SalesData { get; set; } = new();
        public List<decimal> PurchasesData { get; set; } = new();
    }

    public class CategoryDistributionChartViewModel
    {
        public List<string> CategoryNames { get; set; } = new();
        public List<int> ProductCounts { get; set; } = new();
    }
}