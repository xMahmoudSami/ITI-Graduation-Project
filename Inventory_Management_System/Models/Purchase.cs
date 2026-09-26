namespace Inventory_Management_System.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseID { get; set; }

        [Required(ErrorMessage = "Supplier is required")]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Purchase Date is required")]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Total Amount is required")]
        [Range(0, 1000000, ErrorMessage = "Total Amount must be a positive value")]
        public decimal TotalAmount { get; set; }
        

        public Supplier? Supplier { get; set; }
        public ICollection<PurchaseItem> PurchaseItems { get; set; }
    }
}
