namespace Inventory_Management_System.Models
{
    public class PurchaseItem
    {
        [Key]
        public int PurchaseItemID { get; set; }

        [Required(ErrorMessage = "Purchase is required")]
        public int PurchaseID { get; set; }

        [Required(ErrorMessage = "Product is required")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10000, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit Cost is required")]
        [Range(0.01, 100000, ErrorMessage = "Unit Cost must be greater than zero")]
        public decimal UnitCost { get; set; }

        
        public Purchase? Purchase { get; set; }
        public Product? Product { get; set; }

    }
}
