using Inventory_Management_System.Models;

namespace InventoryManagementSystem.Models
{
    public class UniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            ApplicationDbContext context = new ApplicationDbContext();

            string sku = value.ToString();

            Product productFromDatabase = context.Products.FirstOrDefault(p => p.SKU == sku);

            if (productFromDatabase == null)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("SKU must be unique");
        }
    }
}