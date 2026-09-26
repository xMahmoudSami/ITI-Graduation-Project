namespace Inventory_Management_System.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Supplier Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Supplier Name must be between 2 and 100 characters")]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [Required(ErrorMessage = "Contact Name is required")]
        [StringLength(100, ErrorMessage = "Contact Name cannot exceed 100 characters")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }


        public ICollection<SupplierProduct> SupplierProducts { get; set; }
        public ICollection<Purchase> Purchases { get; set; }

    }
}
