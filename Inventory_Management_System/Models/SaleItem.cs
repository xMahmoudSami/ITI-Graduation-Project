namespace Inventory_Management_System.Models
{
    public class SaleItem
    {
        [Key]
        public int SaleItemID { get; set; }

        [Required(ErrorMessage = "Sale is required")]
        public int SaleID { get; set; }

        [Required(ErrorMessage = "Product is required")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10000, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit Price is required")]
        [Range(0.01, 100000, ErrorMessage = "Price must be greater than zero")]
        public decimal UnitPrice { get; set; }

        public Sale? Sale { get; set; }
        public Product? Product { get; set; }
    }
}