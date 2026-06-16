using System.ComponentModel.DataAnnotations;

namespace ComplaintTicketSystem.Models
{
    public class ComplaintModel
    {
        public int ComplaintId { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(100, ErrorMessage = "Subject cannot exceed 100 characters")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public string? Status { get; set; }

        public int? AssignedTo { get; set; }

        public string? AssignedToName { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ResolvedDate { get; set; }

        public string? CategoryName { get; set; }

        public string? UserName { get; set; }
    }
}