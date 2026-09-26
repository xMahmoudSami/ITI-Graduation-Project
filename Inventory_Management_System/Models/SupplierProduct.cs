namespace Inventory_Management_System.Models
{
    public class SupplierProduct
    {
        [Key]
        public int SupplierProductID { get; set; }

        [Required(ErrorMessage = "Supplier is required")]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Product is required")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Supplier SKU is required")]
        [StringLength(50, ErrorMessage = "Supplier SKU cannot exceed 50 characters")]
        public string SupplierSKU { get; set; }

        [Required(ErrorMessage = "Contract Price is required")]
        [Range(0.01, 100000, ErrorMessage = "Contract Price must be greater than zero")]
        public decimal ContractPrice { get; set; }

        [Required(ErrorMessage = "Lead Time is required")]
        [Range(1, 365, ErrorMessage = "Lead time must be at least 1 day")]
        public int LeadTimeDays { get; set; }


        public Supplier? Supplier { get; set; }
        public Product? Product { get; set; }
    }
}
