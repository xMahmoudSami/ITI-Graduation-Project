namespace Inventory_Management_System.Models
{
    public class AIChatLog
    {
        [Key]
        public int LogID { get; set; }

        [Required(ErrorMessage = "User Query is required")]
        [StringLength(1000, ErrorMessage = "Query cannot exceed 1000 characters")]
        public string UserQuery { get; set; }

        [Required(ErrorMessage = "AI Response is required")]
        public string AIResponse { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}
