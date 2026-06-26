using System.ComponentModel.DataAnnotations;

namespace ComplaintTicketSystem.Models
{
    public class ComplaintCategoryModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100, ErrorMessage = "Category Name cannot exceed 100 characters")]
        public string? CategoryName { get; set; }

        public bool IsActive { get; set; }
    }
}