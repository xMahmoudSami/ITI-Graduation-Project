namespace Inventory_Management_System.Models
{
    public class Sale
    {
        [Key]
        public int SaleID { get; set; }

        [Required(ErrorMessage = "Sale Date is required")]
        [DataType(DataType.Date)]
        public DateTime SaleDate { get; set; }

        [Required(ErrorMessage = "Total Amount is required")]
        [Range(0, 1000000, ErrorMessage = "Total Amount must be positive")]
        public decimal TotalAmount { get; set; }

        [StringLength(200, ErrorMessage = "Customer info cannot exceed 200 characters")]
        public string? CustomerInfo { get; set; }


        public ICollection<SaleItem> SaleItems { get; set; }
    }
}
