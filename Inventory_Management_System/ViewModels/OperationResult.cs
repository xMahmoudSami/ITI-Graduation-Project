namespace Inventory_Management_System.ViewModels
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static OperationResult Ok(string message = "") => new() { Success = true, Message = message };
        public static OperationResult Fail(string error) => new() { Success = false, Message = error, Errors = new() { error } };
        public static OperationResult Fail(List<string> errors) => new() { Success = false, Message = errors.FirstOrDefault() ?? "Operation failed.", Errors = errors };
    }

    public class DeleteCheckResult
    {
        public bool CanDelete { get; set; } = true;
        public string EntityName { get; set; } = string.Empty;
        public int EntityID { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string BlockingReason { get; set; } = string.Empty;
        public List<string> Details { get; set; } = new();
        public int RelatedProductsCount { get; set; }
        public int RelatedPurchasesCount { get; set; }
        public int RelatedSalesCount { get; set; }
        public int RelatedMappingsCount { get; set; }
    }
}
